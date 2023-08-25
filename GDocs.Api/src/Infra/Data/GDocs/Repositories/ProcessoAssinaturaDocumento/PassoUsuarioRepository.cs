using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.Repositories.ProcessoAssinaturaDocumento
{
    internal class PassoUsuarioRepository : Repository, IPassoUsuarioRepository
    {
        public PassoUsuarioRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> Salvar(IEnumerable<AssinaturaPassoItemModel> assinaturaPassoItems, CancellationToken cancellationToken)
        {
            var usuariosQuery = assinaturaPassoItems
                .SelectMany(passo => passo.Usuarios)
                .ConvertAll(usuario => $"({usuario.PassoId},'{usuario.Guid}',{usuario.Status.ToInt32()},{(usuario.NotificarFinalizacao ? 1 : 0)},'{usuario.FluxoNotificacao}',{(usuario.AssinarDigitalmente ? 1 : 0)},{(usuario.AssinarFisicamente ? 1 : 0)})");

            var passosIds = assinaturaPassoItems.Select(passo => passo.Id).ToList();

            string sql = $@"                
                MERGE INTO [dbo].[tb_padpu_processo_assinatura_documento_passo_usuario] AS Target
                USING 
                (
	                VALUES {string.Join(",", usuariosQuery)}	
                ) AS Source 
                ([padp_idt]
                ,[padpu_guid_ad]
                ,[sadpu_idt]
                ,[padpu_flg_notificar_finalizacao]
                ,[padpu_notificar_finalizacao_fluxo]
                ,[padpu_flg_assinar_certificado_digital]
                ,[padpu_flg_assinar_fisicamente])
							
                ON Target.[padp_idt] = Source.[padp_idt] AND Target.[padpu_guid_ad] = Source.[padpu_guid_ad]

                --ativa registros que existem no Target e no Source
                WHEN MATCHED AND [padpu_flg_ativo] = 0 THEN
	                UPDATE SET [padpu_flg_ativo] = 1 ,[padpu_dat_atualizacao] = GETDATE() 
                --inativa registros que somente existem no target e não existem no source
                WHEN NOT MATCHED BY SOURCE AND [padp_idt] IN @passosIds AND [padpu_flg_ativo] = 1 THEN
	                UPDATE SET [padpu_flg_ativo] = 0 , [padpu_dat_atualizacao] = getdate() 		                                
                --inseri novos registros que não existem no target e existem no source
                WHEN NOT MATCHED BY TARGET THEN 
	                INSERT 
                        ([padp_idt]
		                ,[padpu_guid_ad]
		                ,[sadpu_idt]
		                ,[padpu_flg_notificar_finalizacao]
                        ,[padpu_notificar_finalizacao_fluxo]
                        ,[padpu_flg_assinar_certificado_digital]
                        ,[padpu_flg_assinar_fisicamente])
                    VALUES
                        ([padp_idt]
		                ,[padpu_guid_ad]		
		                ,[sadpu_idt]
		                ,[padpu_flg_notificar_finalizacao]
                        ,[padpu_notificar_finalizacao_fluxo]
                        ,[padpu_flg_assinar_certificado_digital]
                        ,[padpu_flg_assinar_fisicamente])

                OUTPUT
	                INSERTED.[padpu_idt] AS Id, INSERTED.[padp_idt] AS PassoId, INSERTED.[padpu_guid_ad] AS Guid;";

            var ids = await _db.Connection.QueryAsync<AssinaturaUsuarioModel>(
                new CommandDefinition(
                    commandText: sql,
                    parameters: new
                    {
                        passosIds
                    },
                    cancellationToken: cancellationToken,
                    transaction: Transaction
                )
            );

            assinaturaPassoItems.ForEach(item =>
            {
                item.Usuarios.ForEach(usuario =>
                {
                    usuario.Id = ids.FirstOrDefault(idUsuario => idUsuario.PassoId == usuario.PassoId && idUsuario.Guid == usuario.Guid).Id;
                });
            });

            return assinaturaPassoItems.ToList();
        }


        public async Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> AtualizaStatusFareiEnvio(int passoUsuarioId, bool status, CancellationToken cancellationToken)
             => (await _db.Connection.QueryAsync<AssinaturaPassoItemModel>(
                                new CommandDefinition(
                                    commandText: $@"UPDATE [dbo].[tb_padpu_processo_assinatura_documento_passo_usuario]  
                                                    SET [padpu_flg_farei_envio] = @status 
                                                    OUTPUT INSERTED.padpu_idt AS Id
                                                    WHERE 
	                                                    [padpu_idt] IN (@passoUsuarioId)",
                                    parameters: new
                                    {
                                        passoUsuarioId,
                                        status = status
                                    },
                                    transaction: Transaction,
                                    cancellationToken: cancellationToken
                                )
                            )).ToCollection();


        public async Task<TryException<IEnumerable<AssinaturaPassoAssinanteRepresentanteModel>>> ListarAssinantesERepresentantesPorPadIdEStatusNaoIniciadoEmAndamento(long processoId,bool todosStatus,CancellationToken cancellationToken)
            => (await _db.Connection.QueryAsync<AssinaturaPassoAssinanteRepresentanteModel>(
                new CommandDefinition(
                            commandText: @"SELECT
	                                            padpu.[padpu_idt] PassoId,
	                                            padpu.[padpu_guid_ad] UsuarioAdAssinanteGuid,
	                                            padpu.[padpu_guid_ad_representante] UsuarioAdRepresentanteGuid,
                                                padpu.[padpu_flg_farei_envio] FareiEnvio
                                            FROM
	                                            [dbo].[tb_padpu_processo_assinatura_documento_passo_usuario] padpu
	                                            INNER JOIN [dbo].[tb_padp_processo_assinatura_documento_passo] padp ON padp.[padp_idt] = padpu.[padp_idt]
                                            WHERE
	                                            padpu.[padpu_flg_ativo] = 1
	                                            AND padp.[pad_idt] = @processoId
	                                            AND padpu.[sadpu_idt] IN @statusAssinaveis
                                            ORDER BY
                                                padp.[padp_ordem] ASC",
                        parameters: new
                        {
                            processoId,
                            statusAssinaveis = new int[]
                            {
                                (int)StatusAssinaturaDocumentoPasso.NaoIniciado,
                                (int)StatusAssinaturaDocumentoPasso.EmAndamento,
                                todosStatus? (int)StatusAssinaturaDocumentoPasso.Concluido : 0
                            }
                        },
                        cancellationToken: cancellationToken
                    ))).ToCollection();

        public async Task AtribuirRepresentantes(IEnumerable<AssinaturaPassoAssinanteRepresentanteModel> passosList, CancellationToken cancellationToken)
        {
            var statusAssinaveis = new int[]
                            {
                                (int)StatusAssinaturaDocumentoPasso.NaoIniciado,
                                (int)StatusAssinaturaDocumentoPasso.EmAndamento
                            };

            string sql = $@"MERGE INTO [dbo].[tb_padpu_processo_assinatura_documento_passo_usuario] AS Target
                         USING 
                         (
                            VALUES (@passoId, @usuarioAdRepresentanteGuid)
                         ) AS Source 
                         ([padpu_idt]
                         ,[padpu_guid_ad_representante])

                         ON Target.[padpu_idt] = Source.[padpu_idt] 

                         -- ATUALIZA REPRESENTANTE E DATA DE ATUALIZAÇÃO
                         WHEN MATCHED 
                                 AND [padpu_flg_ativo] = 1
                                 AND [sadpu_idt] IN @statusAssinaveis
                                 AND Target.[padpu_guid_ad] <> Source.[padpu_guid_ad_representante] 
                         THEN
                          UPDATE 
                             SET
                                 [padpu_dat_atualizacao] = GETDATE()
                                ,[padpu_guid_ad_representante] = Source.[padpu_guid_ad_representante];";

            await _db.Connection.ExecuteAsync(
                new CommandDefinition(
                    commandText: sql,
                    parameters: passosList.ConvertAll(passo =>
                            new
                            {
                                passoId = passo.PassoId,
                                usuarioAdRepresentanteGuid = passo.UsuarioAdRepresentanteGuid.HasValue
                                    ? passo.UsuarioAdRepresentanteGuid.Value
                                    : Guid.Empty,
                                statusAssinaveis
                            }),
                    cancellationToken: cancellationToken,
                    transaction: Transaction
                ));
        }

        public async Task<TryException<IEnumerable<AssinaturaPassoAssinanteModel>>> ListarPorPadId(IEnumerable<int> processoAssinaturaDocumentoId, CancellationToken cancellationToken)
            => (await _db.Connection.QueryAsync<AssinaturaPassoAssinanteModel>(
                new CommandDefinition(
                        commandText: @"SELECT
	                                        padpu.[padpu_idt] ProcessoAssinaturaDocumentoId,
	                                        padpu.[padpu_guid_ad] UsuarioAdAssinanteGuid,
	                                        padpu.[padpu_guid_ad_representante] UsuarioAdRepresentanteGuid,
                                            padpu.[sadpu_idt] status,
                                            padpu.[padpu_flg_notificar_finalizacao] NotificarFinalizacao,
                                            padpu.[padpu_dat_assinatura] DataAssinatura,
                                            padpu.[padpu_justificativa] justificativa,
                                            padpu.[padpu_flg_assinar_certificado_digital] AssinarCertificadoDigital,
	                                        padpu.[padpu_flg_assinar_fisicamente] AssinarFisicamente
                                        FROM
	                                        [dbo].[tb_padpu_processo_assinatura_documento_passo_usuario] padpu
	                                        INNER JOIN [dbo].[tb_padp_processo_assinatura_documento_passo] padp ON padp.[padp_idt] = padpu.[padp_idt]
                                            INNER JOIN [dbo].[tb_pad_processo_assinatura_documento] pad ON pad.[pad_idt] = padp.[pad_idt]
                                        WHERE
	                                        padpu.[padpu_flg_ativo] = 1
                                            AND padp.[padp_flg_ativo] = 1
                                            AND pad.[pad_flg_ativo] = 1
	                                        AND padp.[pad_idt] in @processoAssinaturaDocumentoId	                                        
                                        ORDER BY
                                            padp.[padp_ordem] ASC",
                        parameters: new
                        {
                            processoAssinaturaDocumentoId
                        },
                        transaction: Transaction,
                        cancellationToken: cancellationToken
                    ))).ToCollection();

        public async Task<TryException<IEnumerable<AssinaturaPassoAssinanteModel>>> ListarPorPadId(IEnumerable<int> processoAssinaturaDocumentoId, Guid usuarioAd, CancellationToken cancellationToken)
            => (await _db.Connection.QueryAsync<AssinaturaPassoAssinanteModel>(
                new CommandDefinition(
                        commandText: @"SELECT
                                            pad.[pad_idt] ProcessoAssinaturaId,
	                                        padpu.[padpu_idt] ProcessoAssinaturaDocumentoId,
	                                        padpu.[padpu_guid_ad] UsuarioAdAssinanteGuid,
	                                        padpu.[padpu_guid_ad_representante] UsuarioAdRepresentanteGuid,
                                            padpu.[sadpu_idt] status,
                                            padpu.[padpu_flg_notificar_finalizacao] NotificarFinalizacao,
                                            padpu.[padpu_dat_assinatura] DataAssinatura,
                                            padpu.[padpu_justificativa] justificativa,
                                            padpu.[padpu_flg_assinar_certificado_digital] AssinarCertificadoDigital,
	                                        padpu.[padpu_flg_assinar_fisicamente] AssinarFisicamente,
                                            pada.[pada_idt] ProcessoAssinaturaArquivoCorrenteId,
                                            pada.[bin_idt] BinarioId
                                        FROM
	                                        [dbo].[tb_padpu_processo_assinatura_documento_passo_usuario] padpu
	                                        INNER JOIN [dbo].[tb_padp_processo_assinatura_documento_passo] padp ON padp.[padp_idt] = padpu.[padp_idt]
                                            INNER JOIN [dbo].[tb_pad_processo_assinatura_documento] pad ON pad.[pad_idt] = padp.[pad_idt]
                                            INNER JOIN [dbo].[tb_pada_processo_assinatura_documento_arquivo] pada ON pada.[pad_idt] = pad.[pad_idt] AND pada.[pada_flg_ativo] = 1
                                        WHERE
	                                        padpu.[padpu_flg_ativo] = 1
                                            AND padp.[padp_flg_ativo] = 1
                                            AND pad.[pad_flg_ativo] = 1
	                                        AND padp.[pad_idt] in @processoAssinaturaDocumentoId
                                            AND (padpu.padpu_guid_ad = @usuarioAd OR padpu.padpu_guid_ad_representante = @usuarioAd)
                                        ORDER BY
                                            padp.[padp_ordem] ASC",
                        parameters: new
                        {
                            processoAssinaturaDocumentoId,
                            usuarioAd
                        },
                        transaction: Transaction,
                        cancellationToken: cancellationToken
                    ))).ToCollection();

        public async Task<TryException<Return>> AssinarRejeitar(Guid usuarioAdGuid, IEnumerable<int> listaDeprocessoAssinaturaDocumentoId, StatusAssinaturaDocumentoPassoUsuario status, string justificativaRejeicao, CancellationToken cancellationToken)
        {
            await _db.Connection.ExecuteAsync(
                                new CommandDefinition(
                                    commandText: $@"
                                    UPDATE  
										padpu 
									SET 
										padpu_dat_assinatura = getdate(),
										padpu_justificativa =@justificativaRejeicao,
										sadpu_idt = @status
									FROM
										tb_padpu_processo_assinatura_documento_passo_usuario padpu
									INNER JOIN tb_padp_processo_assinatura_documento_passo padp
										ON padp.padp_idt = padpu.padp_idt
									WHERE
										padp.pad_idt in @listaDeprocessoAssinaturaDocumentoId AND
                                        padpu.sadpu_idt = @statusAguardandoAssinatura AND
										padpu_guid_ad_representante=@usuarioAdGuid;


									UPDATE  
										padpu 
									SET 
										padpu_dat_assinatura = getdate(),
										padpu_justificativa =@justificativaRejeicao,
										sadpu_idt = @status,
										padpu_guid_ad_representante = null
									FROM
										tb_padpu_processo_assinatura_documento_passo_usuario padpu
									INNER JOIN tb_padp_processo_assinatura_documento_passo padp
										ON padp.padp_idt = padpu.padp_idt
									WHERE
										padp.pad_idt in @listaDeprocessoAssinaturaDocumentoId AND
                                        padpu.sadpu_idt = @statusAguardandoAssinatura AND
										padpu_guid_ad=@usuarioAdGuid;",
                                    parameters: new
                                    {
                                        listaDeprocessoAssinaturaDocumentoId,
                                        usuarioAdGuid,
                                        status = (int)status,
                                        statusAguardandoAssinatura = (int)StatusAssinaturaDocumentoPassoUsuario.AguardandoAssinatura,
                                        justificativaRejeicao
                                    },
                                    transaction: Transaction,
                                    cancellationToken: cancellationToken
                                ));

            return Return.Empty;
        }

        public async Task<TryException<Return>> InativarUsuariosDosPassos(int processoAssinaturaDocumentoId, CancellationToken cancellationToken)
        {
            await _db.Connection.ExecuteAsync(
               new CommandDefinition(
                   commandText: @"
                                UPDATE 
                                    tb_padpu_processo_assinatura_documento_passo_usuario  
                                SET  
                                    padpu_flg_ativo = 0 
                                WHERE
                                   padp_idt IN(
                                SELECT padp_idt 
                                    FROM tb_padp_processo_assinatura_documento_passo padp
                                WHERE
                                    padp.pad_idt = @Id 
                                    AND padp_flg_ativo = 1 
                                )",
                   transaction: Transaction,
                   cancellationToken: cancellationToken,
                   parameters: new
                   {
                       Id = processoAssinaturaDocumentoId,
                   }
               ));

            return Return.Empty;
        }



    }
}
