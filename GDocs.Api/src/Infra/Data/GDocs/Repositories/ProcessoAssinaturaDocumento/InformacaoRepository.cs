using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.Repositories.ProcessoAssinaturaDocumento
{
    internal class InformacaoRepository : Repository, IInformacaoRepository
    {
        public InformacaoRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<IEnumerable<AssinaturaInformacoesModel>>> Listar(AssinaturaInformacoesFilterModel filtro, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<AssinaturaInformacoesModel>(
                new CommandDefinition(
                    commandText: $@"SELECT 
                                        pad.[pad_idt] Id,
	                                    pad.[pad_titulo] Titulo,
	                                    pad.[pad_descricao] Descricao,
	                                    pad.[pad_numero_documento] NumeroDocumento,
                                        pad.[pad_nome_documento] NomeDocumento,
                                        pad.[pad_flg_destaque] Destaque,
	                                    pad.[sad_idt] Status,
	                                    pad.[pad_dat_criacao] DataCriacao,
	                                    pad.[pad_guid_ad] UsuarioGuidAd,
                                        pad.[pad_flg_certificado_digital] CertificadoDigital,
                                        pad.[pad_flg_assinado_fisicamente] AssinadoNoDocumento,
                                        sad.[sad_descricao] StatusDescricao,
                                        padc.[padc_idt] CategoriaId,
                                        padc.[padc_descricao] CategoriaDescricao
                                    FROM 	
	                                    [dbo].[tb_pad_processo_assinatura_documento] pad
                                        INNER JOIN tb_padc_processo_assinatura_documento_categoria padc ON padc.padc_idt = pad.padc_idt
                                        INNER JOIN tb_sad_status_assinatura_documento sad ON sad.sad_idt = pad.sad_idt
                                    WHERE
	                                    pad.[pad_flg_ativo] = 1
                                        AND (@NumeroDocumento = 0 OR pad.[pad_numero_documento] = @NumeroDocumento)
	                                    AND (@NomeDocumento IS NULL OR pad.[pad_nome_documento] = @NomeDocumento)
	                                    AND (@countStatus = 0 OR pad.[sad_idt] IN @status)
	                                    AND (CONVERT(DATE, pad.[pad_dat_criacao]) BETWEEN CONVERT(DATE, @dataInicio) AND CONVERT(DATE, @dataTermino))
	                                    AND (@countAutores = 0 OR pad.[pad_guid_ad] IN @autores)
                                        AND (@countPadId = 0 OR pad.[pad_idt] IN @padIdList)
                                        AND (@countCategoriaId = 0 OR pad.[padc_idt] IN @categoriaId)
                                    ORDER BY
	                                    pad.[pad_dat_criacao] DESC,
	                                    pad.[pad_flg_destaque] ASC",
                    parameters: new
                    {
                        filtro.NumeroDocumento,
                        filtro.NomeDocumento,
                        countStatus = filtro.Status.Count(),
                        filtro.Status,
                        countAutores = filtro.UsuariosGuidAd.Count(),
                        autores = filtro.UsuariosGuidAd,
                        dataInicio = filtro.DataInicio.GetValueOrDefault(SqlDateTime.MinValue.Value),
                        dataTermino = filtro.DataTermino.GetValueOrDefault(SqlDateTime.MaxValue.Value),
                        countPadId = filtro.ListaDeprocessoAssinaturaDocumentoId.Count(),
                        padIdList = filtro.ListaDeprocessoAssinaturaDocumentoId,
                        countCategoriaId = filtro.CategoriaId.Count(),
                        categoriaId = filtro.CategoriaId
                    },
                    cancellationToken: cancellationToken,
                    transaction: Transaction
                )
            );

            return result?.ToCollection();
        }

        public async Task<TryException<IEnumerable<AssinaturaNomeDocumento>>> ListarNomesDocumentos(string termo, CancellationToken cancellationToken)
            => (await _db.Connection.QueryAsync<AssinaturaNomeDocumento>(
                    new CommandDefinition(
                        commandText: $@"SELECT DISTINCT
	                                        pad.[pad_nome_documento] Nome
                                        FROM 
	                                        [dbo].[tb_pad_processo_assinatura_documento] pad
                                        WHERE
                                            pad.[pad_flg_ativo] = 1
                                            AND LOWER(pad.[pad_nome_documento]) LIKE LOWER(@termo)
                                        ORDER BY
	                                        pad.[pad_nome_documento] ASC",
                        parameters: new {
                            termo = $"%{termo}%"
                        },
                        cancellationToken: cancellationToken
                    )
                ))?.ToCollection();

        public async Task<TryException<AssinaturaInformacoesModel>> Salvar(AssinaturaInformacoesModel assinaturaInformacoesModel, CancellationToken cancellationToken)
        {
            string sql = $@"                
                MERGE INTO [dbo].[tb_pad_processo_assinatura_documento] AS Target
				USING
				(
					VALUES ({assinaturaInformacoesModel.Id}
                            ,'{TratarSqlEscape(assinaturaInformacoesModel.Titulo)}'
                            ,'{TratarSqlEscape(assinaturaInformacoesModel.Descricao)}'
                            ,'{TratarSqlEscape(assinaturaInformacoesModel.NomeDocumento)}'
                            ,{(assinaturaInformacoesModel.NumeroDocumento == null ? "NULL" : assinaturaInformacoesModel.NumeroDocumento.Value.ToString())}
                            ,{ConverterBoolParaInt(assinaturaInformacoesModel.Destaque)}
                            ,'{assinaturaInformacoesModel.UsuarioGuidAd}'
                            ,{assinaturaInformacoesModel.Status.ToInt32()}
                            ,{assinaturaInformacoesModel.CategoriaId}
                            ,{ConverterBoolParaInt(assinaturaInformacoesModel.GerarNumeroDocumento)}
                            ,{ConverterBoolParaInt(assinaturaInformacoesModel.CertificadoDigital)}
                            ,{ConverterBoolParaInt(assinaturaInformacoesModel.AssinadoNoDocumento)})
				) AS Source 
				([pad_idt],[pad_titulo],[pad_descricao],[pad_nome_documento],[pad_numero_documento],[pad_flg_destaque],[pad_guid_ad],[sad_idt],[padc_idt],
                    [pad_flg_gerar_numero_documento],[pad_flg_certificado_digital], [pad_flg_assinado_fisicamente])							
				ON Target.[pad_idt] = Source.[pad_idt]

				--altera registros que existem no Target e no Source
				WHEN MATCHED THEN
					UPDATE SET 
                            [pad_titulo] = Source.[pad_titulo]
                            ,[pad_descricao] = Source.[pad_descricao]
                            ,[pad_nome_documento] = Source.[pad_nome_documento]
                            ,[pad_numero_documento] = Source.[pad_numero_documento]
                            ,[pad_flg_destaque] = Source.[pad_flg_destaque]                                       
                            ,[sad_idt] = Source.[sad_idt]                                                                                
							,[pad_dat_atualizacao] = GETDATE()
                            ,[padc_idt] = Source.[padc_idt]
                            ,[pad_flg_gerar_numero_documento] = Source.[pad_flg_gerar_numero_documento]
                            ,[pad_flg_certificado_digital] = Source.[pad_flg_certificado_digital]
                            ,[pad_flg_assinado_fisicamente] = Source.[pad_flg_assinado_fisicamente]

				--inseri novos registros que não existem no target e existem no source
				WHEN NOT MATCHED BY TARGET THEN 
					INSERT 
                            ([pad_titulo]
							,[pad_descricao]
							,[pad_nome_documento]
							,[pad_numero_documento]
							,[pad_flg_destaque]
							,[pad_guid_ad]
							,[sad_idt]
                            ,[padc_idt]
                            ,[pad_flg_gerar_numero_documento]
                            ,[pad_flg_certificado_digital]
                            ,[pad_flg_assinado_fisicamente])
                        VALUES
                            ([pad_titulo]
							,[pad_descricao]
							,[pad_nome_documento]
							,[pad_numero_documento]
							,[pad_flg_destaque]
							,[pad_guid_ad]
							,[sad_idt]
                            ,[padc_idt]
                            ,[pad_flg_gerar_numero_documento]
                            ,[pad_flg_certificado_digital]
                            ,[pad_flg_assinado_fisicamente])
                OUTPUT
                    INSERTED.[pad_idt];";

            var id = await _db.Connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    commandText: sql,
                    cancellationToken: cancellationToken,
                    transaction: Transaction
                )
            );

            if (assinaturaInformacoesModel.Id == 0)
                assinaturaInformacoesModel.Id = id;

            if (assinaturaInformacoesModel.NumeroDocumento == null && assinaturaInformacoesModel.GerarNumeroDocumento)
            {
                assinaturaInformacoesModel.NumeroDocumento = assinaturaInformacoesModel.Id;
                await this.Salvar(assinaturaInformacoesModel, cancellationToken);
            }

            return assinaturaInformacoesModel;
        }

        public async Task<TryException<IEnumerable<AssinaturaInformacoesModel>>> AtualizarStatusProcessos(IEnumerable<int> processoListId, StatusAssinaturaDocumento status, CancellationToken cancellationToken)
            => (await _db.Connection.QueryAsync<AssinaturaInformacoesModel>(
                                new CommandDefinition(
                                    commandText: $@"UPDATE [dbo].[tb_pad_processo_assinatura_documento]  
                                                    SET [sad_idt] = @status 
                                                    OUTPUT INSERTED.pad_idt AS Id
                                                    WHERE 
	                                                    [pad_idt] IN @processoListId",
                                    parameters: new
                                    {
                                        processoListId,
                                        status = (int)status
                                    },
                                    transaction: Transaction,
                                    cancellationToken: cancellationToken
                                )
                            ))?.ToCollection();

        public async Task<TryException<AssinaturaModel>> ObterProcessoComPassosParaAssinatura(long processoId, CancellationToken cancellationToken)
        {
            var processoModel = new AssinaturaModel();
            var lookupProcessos = new Dictionary<int, AssinaturaModel>();
            var lookupInfos = new Dictionary<int, AssinaturaInformacoesModel>();
            var lookupPassos = new Dictionary<int, AssinaturaPassoItemModel>();
            var lookupAssinantes = new Dictionary<int, AssinaturaUsuarioModel>();

            await _db.Connection.QueryAsync<
                    AssinaturaInformacoesModel,
                    AssinaturaPassoItemModel,
                    AssinaturaUsuarioModel,
                    AssinaturaModel>(
                    new CommandDefinition(
                        commandText: $@"SELECT 
	                                        pad.[pad_idt] Id,
	                                        pad.[pad_titulo] Titulo,
	                                        padp.[padp_idt] Id,
	                                        padp.[padp_ordem] Ordem,
	                                        padp.[sadp_idt] Status,
	                                        sadp.[sadp_descricao] DescricaoStatus,
	                                        padpu.[padpu_idt] Id,
	                                        padpu.[padpu_guid_ad] Guid,
	                                        padpu.[sadpu_idt] Status,
	                                        sadpu.[sadpu_descricao] DescricaoStatus,
                                            padpu.[padpu_dat_assinatura] DataAssinatura,
                                            padpu.[padpu_guid_ad_representante] GuidRepresentante,
                                            padpu.[padpu_justificativa] Justificativa,
                                            padpu.[padpu_flg_notificar_finalizacao] NotificarFinalizacao,
                                            padpu.[padpu_notificar_finalizacao_fluxo] FluxoNotificacao,
                                            padpu.[padpu_flg_farei_envio] FlagFareiEnvio
                                        FROM 
	                                        [dbo].[tb_pad_processo_assinatura_documento] pad
	                                        INNER JOIN [dbo].[tb_padp_processo_assinatura_documento_passo] padp ON padp.[pad_idt] = pad.[pad_idt]
	                                        INNER JOIN [dbo].[tb_sadp_status_assinatura_documento_passo] sadp ON sadp.[sadp_idt] = padp.[sadp_idt]
	                                        INNER JOIN [dbo].[tb_padpu_processo_assinatura_documento_passo_usuario] padpu ON padpu.[padp_idt] = padp.[padp_idt]
	                                        INNER JOIN [dbo].[tb_sadpu_status_assinatura_documento_passo_usuario] sadpu ON sadpu.[sadpu_idt] = padpu.[sadpu_idt]
                                        WHERE
	                                        pad.[pad_flg_ativo] = 1
	                                        AND padp.[padp_flg_ativo] = 1
	                                        AND padpu.[padpu_flg_ativo] = 1
	                                        AND pad.[pad_idt] = @processoId
                                        ORDER BY
											padp.[padp_ordem]",
                        cancellationToken: cancellationToken,
                        parameters: new
                        {
                            processoId
                        }
                    ),
                    map: (info, passo, assinante) =>
                    {
                        if (!lookupProcessos.TryGetValue(1, out AssinaturaModel processoEntry))
                        {
                            processoEntry = processoModel;
                            lookupProcessos.Add(1, processoModel);
                        }

                        if (!lookupInfos.TryGetValue(info.Id, out AssinaturaInformacoesModel infoEntry))
                        {
                            infoEntry = info;
                            lookupInfos.Add(info.Id, info);
                            processoEntry.Informacoes = infoEntry;
                        }

                        if (!lookupPassos.TryGetValue(passo.Id, out AssinaturaPassoItemModel passoEntry))
                        {
                            passoEntry = passo;
                            lookupPassos.Add(passo.Id, passo);
                            processoEntry.Passos.Itens.Add(passoEntry);
                        }

                        if (!lookupAssinantes.TryGetValue(assinante.Id, out AssinaturaUsuarioModel assinanteEntry))
                        {
                            assinanteEntry = assinante;
                            lookupAssinantes.Add(assinante.Id, assinante);
                            passoEntry.Usuarios.Add(assinanteEntry);
                        }

                        return processoModel;
                    },
                    splitOn: "Id,Id,Id"
                );


            return lookupProcessos.Values.FirstOrDefault();
        }

        private string TratarSqlEscape(string texto)
            => texto?.SqlEscape();

        private int ConverterBoolParaInt(bool valor)
            => valor ? 1 : 0;

        public async Task<TryException<Return>> InativarInformacao(int processoAssinaturaDocumentoId, CancellationToken cancellationToken)
        {
            await _db.Connection.ExecuteAsync(
               new CommandDefinition(
                   commandText: @"
                                UPDATE 
                                    tb_pad_processo_assinatura_documento 
                                SET  
                                    pad_flg_ativo = 0 
                                WHERE
                                    pad_idt = @Id
                                    AND pad_flg_ativo = 1",
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
