using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.Repositories.ProcessoAssinaturaDocumento
{
    internal class PassoRepository : Repository, IPassoRepository
    {
        public PassoRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> Salvar(int processoAssinaturaDocumentoId, IEnumerable<AssinaturaPassoItemModel> assinaturaPassoItems, CancellationToken cancellationToken)
        {
            string sql = $@"                
                MERGE INTO [dbo].[tb_padp_processo_assinatura_documento_passo] AS Target
                USING 
                (
	                VALUES { string.Join(",", assinaturaPassoItems.ConvertAll(item => $"({processoAssinaturaDocumentoId},{item.Ordem},{item.Status.ToInt32()},{(item.AguardarTodosUsuarios ? 1 : 0)})"))}
                ) AS Source 
                ([pad_idt]
                ,[padp_ordem]
                ,[sadp_idt]
                ,[padp_flg_aguardar_todos_usuarios])
							
                ON Target.[pad_idt] = Source.[pad_idt] AND Target.[padp_ordem] = Source.[padp_ordem]

                --ativa registros que existem no Target e no Source
                WHEN MATCHED AND [padp_flg_ativo] = 0 THEN
	                UPDATE SET [padp_flg_ativo] = 1 ,[padp_dat_atualizacao] = GETDATE() 
                --inativa registros que somente existem no target e não existem no source
                WHEN NOT MATCHED BY SOURCE AND [pad_idt] = @processoAssinaturaDocumentoId AND [padp_flg_ativo] = 1 THEN
	                UPDATE SET [padp_flg_ativo] = 0 , [padp_dat_atualizacao] = getdate() 		                                
                --inseri novos registros que não existem no target e existem no source
                WHEN NOT MATCHED BY TARGET THEN 
	                INSERT 
                        ([pad_idt]
		                ,[padp_ordem]
		                ,[sadp_idt]
                        ,[padp_flg_aguardar_todos_usuarios])
                    VALUES
                        ([pad_idt]
		                ,[padp_ordem]		
		                ,[sadp_idt]
                        ,[padp_flg_aguardar_todos_usuarios])

                OUTPUT
	                INSERTED.[padp_idt] AS Id, INSERTED.[padp_ordem] AS Ordem;";

            var ids = await _db.Connection.QueryAsync<AssinaturaPassoItemModel>(
                new CommandDefinition(
                    commandText: sql,
                    parameters: new
                    {
                        processoAssinaturaDocumentoId
                    },
                    cancellationToken: cancellationToken,
                    transaction: Transaction
                )
            );

            assinaturaPassoItems = assinaturaPassoItems.ForEach(item =>
            {
                item.Id = ids.FirstOrDefault(id => id.Ordem == item.Ordem).Id;
                item.Usuarios.ForEach(usuario => usuario.PassoId = item.Id);
            });

            return assinaturaPassoItems?.ToCollection();
        }

        public async Task<TryException<Return>> InativarPassos(int processoAssinaturaDocumentoId, CancellationToken cancellationToken)
        {
            await _db.Connection.ExecuteAsync(
               new CommandDefinition(
                   commandText: @"
                                UPDATE 
                                    tb_padp_processo_assinatura_documento_passo 
                                SET  
                                    padp_flg_ativo = 0 
                                WHERE
                                    pad_idt = @Id
                                    and padp_flg_ativo = 1",
                   transaction: Transaction,
                   cancellationToken: cancellationToken,
                   parameters: new
                   {
                       Id = processoAssinaturaDocumentoId,
                   }
               ));

            return Return.Empty;
        }

        public async Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> ObterPassosEhUsuariosPorId(int processoAssinaturaDocumentoId, CancellationToken cancellationToken)
        {
            var listaPasso = new Dictionary<int, AssinaturaPassoItemModel>();
            var result = (await _db.Connection.QueryAsync<AssinaturaPassoItemModel, AssinaturaUsuarioModel, AssinaturaPassoItemModel>(
             new CommandDefinition(
                 commandText: @"SELECT  
		                            padp.padp_idt [Id],
		                            padp.padp_ordem [Ordem],
		                            padp.padp_flg_aguardar_todos_usuarios [AguardarTodosUsuarios],
		                            padp.sadp_idt  [Status]
    
                                    ,padpu.padpu_idt [Id_padpu]
		                            ,padpu.padpu_idt [Id]
		                            ,padpu.padp_idt [PassoId]
		                            ,padpu.padpu_guid_ad [Guid]
		                            ,padpu.padpu_flg_notificar_finalizacao [NotificarFinalizacao]
		                            ,padpu.padpu_notificar_finalizacao_fluxo [FluxoNotificacao]
		                            ,padpu.sadpu_idt [Status]
		                            ,padpu.padpu_dat_assinatura [DataAssinatura]
		                            ,padpu.padpu_guid_ad_representante [GuidRepresentante]
		                            ,padpu.padpu_justificativa [Justificativa]
		                            ,padpu.padpu_flg_assinar_certificado_digital [AssinarDigitalmente]
		                            ,padpu.padpu_flg_assinar_fisicamente [AssinarFisicamente]
		                            ,padpu.padpu_flg_farei_envio [FlagFareiEnvio]
                                FROM
		                            tb_padp_processo_assinatura_documento_passo padp 
                                    INNER JOIN tb_padpu_processo_assinatura_documento_passo_usuario padpu on padp.padp_idt = padpu.padp_idt
                                WHERE 
		                            padp.pad_idt = @id
                                    AND padpu.padpu_notificar_finalizacao_fluxo  NOT IN ('DIR-MESMO-PASSO')",
                                    parameters: new { id =processoAssinaturaDocumentoId },
                                    cancellationToken: cancellationToken,
                                    transaction: Transaction
                    ),
                    splitOn: "Id,Id_padpu",
                    map: (passoItemMap, itemMap) =>
                    {
                        if (!listaPasso.TryGetValue(passoItemMap.Id, out var material))
                        {
                            material = passoItemMap;
                            listaPasso.Add(material.Id, material);
                        }

                        if (itemMap.PassoId == material.Id)
                        {
                           material.DefinirUsuario(itemMap);
                        }


                        return material;
                    }
                )).ToCollection();


            return result.GroupBy(passo => passo.Id).Select(x => x.First()).ToCollection();
        }

    }
}
