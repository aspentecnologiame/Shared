using Dapper;
using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.DocumentoFI1548.Enum;
using ICE.GDocs.Infra.CrossCutting.Models.DocumentoFI1548.ViewModel;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.Data.Core.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories
{
    internal class DocumentoFI1548Repository : Repository, IDocumentoFI1548Repository
    {
        private readonly IConfiguration _config;
        public DocumentoFI1548Repository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork,
            IConfiguration config)
            : base(db, unitOfWork)
        {
            _config = config;
        }

        public async Task<TryException<DocumentoFI1548Model>> Salvar(DocumentoFI1548Model docFI1548, CancellationToken cancellationToken)
        {
            string sqlCmd = $@"  
                            DECLARE @idDocumento int                            

                            MERGE INTO [dbo].[tb_dfi_documento_fi1548] AS Target
							USING 
							(
							    VALUES (@Id, @Descricao, @Referencia, @Valor, @TipoPagamento, @QuantidadeParcelas, @Status, @AutorId, @Ativo, @DataLiquidacao, @BinarioId, @Numero, @MoedaSimbolo, @Fornecedor, @PrazoEntrega, @VencimentoPara, @ReferenciaSubstituto)
							) AS Source 
							([dfi_idt]	,[dfi_descricao] ,[dfi_referencia] ,[dfi_valor_total] ,[dfi_pagamento] ,[dfi_quantidade_parcelas] ,[sdo_idt] ,[dfi_guid_usuario_ad] ,[dfi_flg_ativo] ,[dfi_dat_liquidacao]
                            ,[bin_idt] ,[dfi_numero] ,[dfi_moeda_simbolo] ,[dfi_fornecedor] ,[dfi_prazo_entrega] ,[dfi_vencimento_pagamento_unico], [dfi_referencia_substituto])
							
							ON Target.[dfi_idt] = Source.[dfi_idt]
							--altera registros que existem no Target e no Source
							WHEN MATCHED THEN
								UPDATE SET 
                                        [dfi_descricao] = Source.[dfi_descricao]
                                       ,[dfi_referencia] = Source.[dfi_referencia]
                                       ,[dfi_valor_total] = Source.[dfi_valor_total]
                                       ,[dfi_pagamento] = Source.[dfi_pagamento]
                                       ,[dfi_quantidade_parcelas] = Source.[dfi_quantidade_parcelas]
                                       ,[sdo_idt] = Source.[sdo_idt]
                                       ,[dfi_flg_ativo] = Source.[dfi_flg_ativo]
                                       ,[dfi_dat_atualizacao] = GETDATE()
                                       ,[bin_idt] = Source.[bin_idt]
                                       ,[dfi_numero] = Source.[dfi_numero]
                                       ,@idDocumento = Source.[dfi_idt]
                                       ,[dfi_moeda_simbolo] = Source.[dfi_moeda_simbolo]
                                       ,[dfi_fornecedor] = Source.[dfi_fornecedor]
                                       ,[dfi_prazo_entrega] = Source.[dfi_prazo_entrega]
                                       ,[dfi_vencimento_pagamento_unico] = Source.[dfi_vencimento_pagamento_unico]
                                       ,[dfi_referencia_substituto] = Source.[dfi_referencia_substituto]

							--inseri novos registros que não existem no target e existem no source
							WHEN NOT MATCHED BY TARGET THEN 
								INSERT 
                                       ([dfi_descricao], [dfi_referencia] ,[dfi_valor_total] ,[dfi_pagamento] ,[dfi_quantidade_parcelas] ,[sdo_idt] ,[dfi_guid_usuario_ad] ,[dfi_flg_ativo]
                                       ,[dfi_dat_criacao] ,[dfi_dat_atualizacao] ,[bin_idt] ,[dfi_numero] ,[dfi_moeda_simbolo] ,[dfi_fornecedor] ,[dfi_prazo_entrega] ,[dfi_vencimento_pagamento_unico],
                                        [dfi_referencia_substituto])
                                 VALUES
                                       ([dfi_descricao] ,[dfi_referencia] ,[dfi_valor_total] ,[dfi_pagamento] ,[dfi_quantidade_parcelas] ,[sdo_idt] ,[dfi_guid_usuario_ad] ,[dfi_flg_ativo]
                                       ,GETDATE() ,GETDATE() ,[bin_idt] ,[dfi_numero] ,[dfi_moeda_simbolo] ,[dfi_fornecedor] ,[dfi_prazo_entrega] ,[dfi_vencimento_pagamento_unico],
                                        [dfi_referencia_substituto]);

                                        IF @idDocumento IS NULL
                                        BEGIN
                                            SET @idDocumento = CAST(SCOPE_IDENTITY() as [int]);
                                        END

                                        SELECT @idDocumento; ";

            var resultCmd = await _db.Connection.ExecuteScalarAsync(
                new CommandDefinition(
                    commandText: sqlCmd,
                    parameters: new 
                    {
                        docFI1548.Id,
                        Descricao = docFI1548.Descricao?.SqlEscape(),
                        Referencia = docFI1548.Referencia?.SqlEscape(),
                        Valor = $"{docFI1548.Valor: 0.##}".Replace(",", "."),
                        docFI1548.TipoPagamento,
                        docFI1548.QuantidadeParcelas,
                        Status = (int)docFI1548.Status,
                        docFI1548.AutorId,
                        Ativo = docFI1548.Ativo ? 1 : 0,
                        DataLiquidacao = docFI1548.DataLiquidacao.HasValue ? docFI1548.DataLiquidacao.Value.ToString("yyyy-MM-dd") : null,
                        docFI1548.BinarioId,
                        docFI1548.Numero,
                        MoedaSimbolo = docFI1548.MoedaSimbolo?.SqlEscape(),
                        Fornecedor = docFI1548.Fornecedor?.SqlEscape(),
                        PrazoEntrega = docFI1548.PrazoEntrega?.SqlEscape(),
                        docFI1548.VencimentoPara,
                        docFI1548.ReferenciaSubstituto
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );
            int.TryParse(resultCmd?.ToString(), out int id);

            docFI1548.DefinirId(id);

            return docFI1548;
        }

        public async Task<TryException<IEnumerable<DocumentoFI1548Model>>> Listar(DocumentoFI1548FilterModel filtro, CancellationToken cancellationToken)
        {
            var resultQuery = await _db.Connection.QueryAsync<DocumentoFI1548Model>(
                new CommandDefinition(
                    commandText: $@"SELECT 
                                   dfi.[dfi_idt] AS Id
                                  ,dfi.[dfi_descricao] AS Descricao
                                  ,dfi.[dfi_referencia] AS Referencia
                                  ,dfi.[dfi_valor_total] AS Valor
                                  ,dfi.[dfi_pagamento] AS TipoPagamento
                                  ,dfi.[dfi_quantidade_parcelas] AS QuantidadeParcelas
                                  ,dfi.[sdo_idt] AS Status
								  ,sdo.[sdo_des] AS DescricaoStatus
                                  ,dfi.[dfi_guid_usuario_ad] AS AutorId
                                  ,dfi.[dfi_flg_ativo] AS Ativo
                                  ,dfi.[dfi_dat_criacao] AS DataCriacao
                                  ,dfi.[dfi_dat_atualizacao] AS DataAtualizacao
                                  ,dfi.[dfi_dat_liquidacao] AS DataLiquidacao
                                  ,dfi.[bin_idt] AS BinarioId
                                  ,dfi.[dfi_numero] AS Numero
                                  ,dfi.[dfi_moeda_simbolo] AS MoedaSimbolo 
                                  ,dfi.[dfi_fornecedor] AS Fornecedor 
                                  ,dfi.[dfi_prazo_entrega] AS PrazoEntrega 
                                  ,dfi.[dfi_vencimento_pagamento_unico] AS VencimentoPara 
                                  ,dfi.[dfi_referencia_substituto] AS ReferenciaSubstituto
				                  ,(SELECT pad.pad_flg_destaque 
                                        FROM tb_pado_processo_assinatura_documento_origem  pado
									    INNER JOIN tb_pad_processo_assinatura_documento pad on pad.pad_idt = pado.pad_idt
									WHERE
									    pado.pado_origem_idt = @id
                                        AND pado.pado_flg_ativo = 1
									    AND pado.pado_origem_nome = 'Fi1548StatusPagamento')
                                    AS Destaque
                              FROM 
								   [dbo].[tb_dfi_documento_fi1548] dfi
								   INNER JOIN [dbo].[tb_sdo_status_documento] sdo ON sdo.sdo_idt = dfi.sdo_idt
                              WHERE
                                   [dfi_flg_ativo] = 1
                                   AND (@id = 0 OR [dfi_idt] = @id)
                                   AND (@numero = 0 OR [dfi_numero] = @numero)
                                   AND (@countTipoPagamento = 0 OR [dfi_pagamento] IN @tiposPagamento)
                                   AND (@countAutores = 0 OR [dfi_guid_usuario_ad] IN @autores)
                                   AND (@countStatus = 0 OR dfi.[sdo_idt] IN @status)
                                   AND (CONVERT(DATE, [dfi_dat_criacao]) BETWEEN CONVERT(DATE, @dataInicio) AND CONVERT(DATE, @dataTermino))
	                               AND (
                                       @usuarioLogadoAd = '00000000-0000-0000-0000-000000000000'
                                       OR
		                               (@usuarioLogadoAd <> [dfi_guid_usuario_ad] AND dfi.[sdo_idt] <> @statusEmConstrucao)
		                               OR
		                               @usuarioLogadoAd = [dfi_guid_usuario_ad]
	                               )
                              ORDER BY
                                    {filtro.Ordenar.OrdenarPor}",
                    parameters: new
                    {
                        id = filtro.Id,
                        numero = filtro.Numero,
                        tiposPagamento = filtro.TiposPagamento,
                        autores = filtro.AutoresADId,
                        dataInicio = filtro.DataInicio.GetValueOrDefault(SqlDateTime.MinValue.Value),
                        dataTermino = filtro.DataTermino.GetValueOrDefault(SqlDateTime.MaxValue.Value),
                        status = filtro.Status,
                        countAutores = filtro.AutoresADId.Count(),
                        countTipoPagamento = filtro.TiposPagamento.Count(),
                        countStatus = filtro.Status.Count(),
                        usuarioLogadoAd = filtro.UsuarioLogadoAd,
                        statusEmConstrucao = DocumentoFI1548Status.EmConstrucao.ToInt32()
                    },
                    cancellationToken: cancellationToken
                )
            );

            return resultQuery?.ToCollection();
        }

        public async Task<TryException<IEnumerable<DocumentoFI1548StatusModel>>> ListarStatusPagamento(CancellationToken cancellationToken)
            => (await _db.Connection.QueryAsync<DocumentoFI1548StatusModel>(
                new CommandDefinition(
                    commandText: $@"SELECT
	                                   sdo.[sdo_idt] [Id],
                                       sdo.[sdo_des] [Descricao]
                                    FROM 
	                                    [dbo].[tb_sdo_status_documento] sdo
                                    WHERE
	                                    sdo.[sdo_flg_ativo] = 1",
                    cancellationToken: cancellationToken
                ))).ToCollection();

        public async Task<TryException<IEnumerable<DropDownCustonModel>>> ListarSubstitutoRejeitadoAssinatura(Guid UsuarioLogadoAd, CancellationToken cancellationToken)
    => (await _db.Connection.QueryAsync<DropDownCustonModel>(
        new CommandDefinition(
            commandText: $@"	         SELECT 			
										    dfi.dfi_idt as Id , 
											dfi.dfi_descricao as Descricao,
											CONVERT(varchar(10),  dfi_numero) +' - '+ dfi_descricao as Valor,
											FlgEmAprovacaoCiencia = 0
										 FROM [tb_dfi_documento_fi1548] dfi
										  join tb_pado_processo_assinatura_documento_origem pado on pado.pado_origem_idt = dfi.dfi_idt
										  join tb_pad_processo_assinatura_documento pad on pad.pad_idt = pado.pad_idt 
										  join tb_padp_processo_assinatura_documento_passo padp on padp.pad_idt = pad.pad_idt 
										  join tb_padpu_processo_assinatura_documento_passo_usuario  padu on padu.padp_idt = padp.padp_idt 
									     WHERE 
										  padu.sadpu_idt = @statusPasso
										  and dfi.dfi_idt not in (select dfi_referencia_substituto from tb_dfi_documento_fi1548 where dfi_referencia_substituto is not null)
                                          and dfi_dat_atualizacao > dateadd(DD,-10,getdate())
                                          and dfi.dfi_guid_usuario_ad = @usuarioLogadoAd
                                          and dfi.sdo_idt = @statusRejeitado
                                            group by dfi.dfi_idt,dfi.dfi_descricao, dfi_numero", parameters: new
            {
                usuarioLogadoAd = UsuarioLogadoAd,
                statusPasso = StatusAssinaturaDocumentoPassoUsuario.Rejeitado,
                statusRejeitado = DocumentoFI1548Status.Reprovado

            },
                cancellationToken: cancellationToken
        ))).ToCollection();

                public async Task<TryException<IEnumerable<DropDownCustonModel>>> ListarSubstitutoAprovadosPorCiencia(Guid UsuarioLogadoAd, CancellationToken cancellationToken)
        => (await _db.Connection.QueryAsync<DropDownCustonModel>(
        new CommandDefinition(
            commandText: $@"	         SELECT  
											dfi.dfi_idt as Id , 
											dfi.dfi_descricao as Descricao,
											CONVERT(varchar(10),  dfi_numero) +' - '+ dfi_descricao as Valor,
											FlgEmAprovacaoCiencia = 0
										 FROM
                                         tb_dfi_documento_fi1548 dfi 
										 join tb_dfisoc_solicitacao_ciencia_fi1548 soc on soc.dfi_idt = dfi.dfi_idt
										 WHERE 
										 soc.dfisci_idt = @statusCiencia
										 and soc.dfitci_idt = @statusTipoCiencia
									     and dfi.dfi_idt not in (select dfi_referencia_substituto from tb_dfi_documento_fi1548 where dfi_referencia_substituto is not null)
                                         and dfi_dat_atualizacao > dateadd(DD,-10,getdate())
                                         and dfi.dfi_guid_usuario_ad = @usuarioLogadoAd
                                            group by dfi.dfi_idt,dfi.dfi_descricao, dfi_numero
                    ", parameters: new
            {
                usuarioLogadoAd = UsuarioLogadoAd,
                statusCiencia = StatusCienciaFi1548.Concluido,
                statusTipoCiencia = TipoCienciaFi1548.Cancelamento,
            },
                cancellationToken: cancellationToken
        ))).ToCollection();



                    public async Task<TryException<IEnumerable<DropDownCustonModel>>> ListarSubstitutoAguardandoCiencia(Guid UsuarioLogadoAd, CancellationToken cancellationToken)
            => (await _db.Connection.QueryAsync<DropDownCustonModel>(
            new CommandDefinition(
            commandText: $@"		   SELECT  
											dfi.dfi_idt as Id , 
											dfi.dfi_descricao as Descricao,
											CONVERT(varchar(10),  dfi_numero) +' - '+ dfi_descricao as Valor,
											FlgEmAprovacaoCiencia = 1
										 from tb_dfi_documento_fi1548 dfi 
							            WHERE 
										 dfi.sdo_idt = @statusPagamento
										 and dfi.dfi_idt not in (select dfi_referencia_substituto from tb_dfi_documento_fi1548 where dfi_referencia_substituto is not null)
                                         and dfi_dat_atualizacao > dateadd(DD,-10,getdate())
                                         and dfi.dfi_guid_usuario_ad = @usuarioLogadoAd
                                            group by dfi.dfi_idt,dfi.dfi_descricao, dfi_numero", parameters: new
            {
             usuarioLogadoAd = UsuarioLogadoAd,
             statusPagamento = DocumentoFI1548Status.AprovacaoCiencia,
            },
             cancellationToken: cancellationToken
            ))).ToCollection();

        public async Task<TryException<int>> ObterReferenciaSubstituto(long processoAssinaturaOrigem, CancellationToken cancellationToken)
  => (await _db.Connection.QueryAsync<int>(
         new CommandDefinition(
                 commandText: @"
                               select top 1
                                    dfi2.dfi_numero, *
                                    
                                    from tb_pad_processo_assinatura_documento pad
                                    inner join tb_dfi_documento_fi1548 dfi on dfi.dfi_numero = pad.pad_numero_documento
                                    inner join tb_dfi_documento_fi1548 dfi2 on dfi.dfi_referencia_substituto = dfi2.dfi_idt
                                    inner join tb_pad_processo_assinatura_documento pad2 on pad2.pad_numero_documento = dfi2.dfi_numero and pad2.sad_idt in (4,5)
                                where pad.pad_idt = @processoAssinaturaOrigem
                                order by pad2.pad_dat_atualizacao desc",
                 parameters: new
                 {
                     processoAssinaturaOrigem
                 },
                 transaction: Transaction,
                 cancellationToken: cancellationToken
             ))).FirstOrDefault();

        public async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPorUsuario(Guid activeDirectoryId, CancellationToken cancellationToken)
        {
            var processoCienciaFI1548 = _config.GetValue("DocumentoFI1548:ProcessoAssinaturaDocumentoOrigemNome", string.Empty);

            var resultAssinatura = await _db.Connection.QueryAsync<ProcessoAssinaturaDocumentoModel>(
                    new CommandDefinition(
                        commandText: $@"SELECT 
	                                        dfisoc.dfisoc_idt as [CienciaId],
                                            dfisoc.dfisoc_des_observacao AS [Observacao],
	                                        pad.pad_idt AS [Id],
                                            pad.pad_titulo AS [Titulo],
                                            pad.pad_descricao AS [Descricao],
                                            pad.pad_nome_documento AS [NomeDocumento],
                                            pad.pad_flg_destaque AS [Destaque],
                                            pad.pad_guid_ad AS [AutorId],
                                            pad.sad_idt AS [StatusId],
                                            pad.pad_dat_criacao AS [DataCriacao],
                                            pad.pad_numero_documento AS [Numero],
                                            pad.padc_idt AS CategoriaId,
                                            padc.padc_descricao AS DescricaoCategoria,
                                            null [AssinarCertificadoDigital], 
                                            null [AssinarFisicamente],
                                            null [AssinadoPorMimVisivel],
                                            1 AS [UsuarioDaConsultaAssinou],
                                            convert(int, isnull(
		                                            (select top 1
                                                            case 
																when aux.dfiscua_flg_rejeitado = 0 then  2
																when aux.dfiscua_flg_rejeitado = 1 then  4
															 end 
	                                              from 
	                                                tb_dfiscua_solicitacao_ciencia_usuario_aprovacao_fi1548 aux 
	                                              where 
	                                                aux.dfisoc_idt = dfisoc.dfisoc_idt and
		                                            aux.dfiscua_usu_guid_ad != dfiscua.dfiscua_usu_guid_ad and
		                                            aux.dfiscua_dat_aprovacao is not null and
		                                            aux.dfiscua_flg_ativo = 1
	                                            ), 1)) AS [StatusPassoAtualId]
                                        FROM 
	                                        tb_dfisoc_solicitacao_ciencia_fi1548 dfisoc INNER JOIN  tb_dfiscua_solicitacao_ciencia_usuario_aprovacao_fi1548 dfiscua
								                                            on dfisoc.dfisoc_idt = dfiscua.dfisoc_idt and
									                                           dfiscua.dfiscua_flg_ativo = 1
								                                           INNER JOIN tb_pado_processo_assinatura_documento_origem pado
								                                            on pado.pado_origem_idt = dfisoc.dfi_idt and
									                                           pado_origem_nome = @processoCienciaFI1548
								                                           INNER JOIN tb_pad_processo_assinatura_documento pad
								                                            on pad.pad_idt = pado.pad_idt
								                                           INNER JOIN tb_padc_processo_assinatura_documento_categoria padc
								                                            on padc.padc_idt = pad.padc_idt
                                        WHERE
	                                        dfisoc.dfisoc_flg_ativo = 1 and
                                            dfisoc.dfisci_idt = 1 and
	                                        dfiscua.dfiscua_usu_guid_ad = @activeDirectoryId and
	                                        dfiscua.dfiscua_dat_aprovacao is null
                                        ORDER BY 
                                            pad.pad_numero_documento",
                        parameters: new
                        {
                            activeDirectoryId,
                            processoCienciaFI1548
                        },
                        cancellationToken: cancellationToken
                    )
                );

            return resultAssinatura.ToList();
        }

        public async Task<TryException<int>> Inserir(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, CancellationToken cancellationToken)
        {
            var documentoFI1548CienciaId = await _db.Connection.QueryFirstAsync<int>(new CommandDefinition(
                    commandText: @"
                         INSERT INTO [dbo].[tb_dfisoc_solicitacao_ciencia_fi1548]
			                   (
                                dfi_idt,
                                dfitci_idt,
                                dfisci_idt,
                                dfisoc_usu_guid_ad,
                                dfisoc_des_observacao,
                                dfisoc_flg_ativo,
                                dfisoc_dat_criacao,
                                dfisoc_dat_atualizacao
                               )
		                 VALUES
			                   (
                                @DocumentoFI1548Id
			                   ,@IdTipoCiencia
			                   ,@IdStatusCiencia
			                   ,@IdUsuario
			                   ,@Observacao
			                   ,@FlgAtivo
			                   ,GETDATE()
			                   ,GETDATE()
                            );

                            SELECT CAST(SCOPE_IDENTITY() as [int]);",
                    parameters: documentoFI1548CienciaModel,
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                ));

            var aprovadoresFI1548 = await InserirAprovadoresFI1548(documentoFI1548CienciaId, cancellationToken);

            if (aprovadoresFI1548.IsFailure)
                return aprovadoresFI1548.Failure;

            return documentoFI1548CienciaId;
        }

        public async Task<TryException<DocumentoFI1548CienciaModel>> ObterCienciaPorId(int idSolicitacaoCiencia, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<DocumentoFI1548CienciaModel>(
                    new CommandDefinition(
                        commandText: $@"SELECT 
                                            dfisoc.dfisoc_idt as [CienciaId],
                                            dfisoc.dfi_idt as [DocumentoFI1548Id],
                                            dfisoc.dfitci_idt as [IdTipoCiencia],
                                            dfitci.dfitci_des as [TipoCiencia],
                                            dfisoc.dfisci_idt as [IdStatusCiencia],
                                            dfisci.dfisci_des as [StatusCiencia],
                                            dfi.dfi_guid_usuario_ad as [IdUsuario],
                                            dfisoc.dfisoc_des_observacao as [Observacao],
                                            dfisoc.dfisoc_flg_ativo as [FlgAtivo],
                                            dfi.dfi_numero as [DocumentoFI1548Numero],
                                            dfisoc.dfisoc_des_observacao as [MotivoSolicitacao],
                                            dfi.dfi_fornecedor as [Fornecedor],
                                            dfi.dfi_pagamento as [Pagamento],
                                            dfi.dfi_moeda_simbolo as [MoedaSimbolo],
                                            dfi.dfi_valor_total as [ValorTotal],
                                            dfi.dfi_descricao as [Justificativa]
                                        FROM 
	                                        tb_dfisoc_solicitacao_ciencia_fi1548 dfisoc INNER JOIN tb_dfitci_tipo_ciencia_fi1548 dfitci
                                                                              on dfisoc.dfitci_idt = dfitci.dfitci_idt
                                                                           INNER JOIN tb_dfisci_status_ciencia_fi1548 dfisci
                                                                              on dfisoc.dfisci_idt = dfisci.dfisci_idt
                                                                           INNER JOIN tb_dfi_documento_fi1548 dfi 
                                                                              on dfisoc.dfi_idt = dfi.dfi_idt
                                        WHERE
                                            dfisoc.dfisoc_idt = @idSolicitacaoCiencia
	                                        and dfisoc.dfisoc_flg_ativo = 1",
                        parameters: new
                        {
                            idSolicitacaoCiencia
                        },
                        cancellationToken: cancellationToken
                    )
                );

            return result.FirstOrDefault();
        }

        public async Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarObsUsuarioPorCiencia(int idSolicitacaoCiencia, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<CienciaUsuarioAprovacaoModel>(
                    new CommandDefinition(
                        commandText: $@"
		                    	  SELECT
                                     u.dfiscua_idt as Id
                                    ,u.dfiscua_usu_guid_ad as UsuarioGuid
                                    ,u.dfiscua_des_observacao as Observacao
                                    ,u.dfiscua_dat_aprovacao as DataAprovacao
                                    ,u.dfiscua_flg_rejeitado as FlgRejeitado
                                 FROM 
                                    tb_dfiscua_solicitacao_ciencia_usuario_aprovacao_fi1548 u
                                 WHERE
                                    u.dfiscua_flg_rejeitado = 1
                                    AND u.dfisoc_idt = @idSolicitacaoCiencia ",
                        parameters: new
                        {
                            idSolicitacaoCiencia,
                        },
                        cancellationToken: cancellationToken
                    )
                );

            return result.ToCollection();
        }

        public async Task<TryException<int>> RegistroCiencia(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, int idCienciaAprovador, CancellationToken cancellationToken)
        {
            var resultRegistro = await _db.Connection.QueryFirstOrDefaultAsync<int>(
                                new CommandDefinition(
                                    commandText: $@"UPDATE 
                                                 tb_dfiscua_solicitacao_ciencia_usuario_aprovacao_fi1548
                                                 SET 
                                                 dfiscua_dat_aprovacao = GETDATE(),
                                                 dfiscua_flg_rejeitado = @flagRejeitar,
                                                 dfiscua_des_observacao = @observacao
                                                 WHERE 
                                                 dfiscua_idt = @IdAprovacao",
                                    parameters: new
                                    {
                                        IdAprovacao = idCienciaAprovador,
                                        flagRejeitar = documentoFI1548CienciaModel.Ciente,
                                        observacao = documentoFI1548CienciaModel.MotivoRejeicao,

                                    },
                                    cancellationToken: cancellationToken,
                                    transaction: Transaction
                                )
                            );

            return resultRegistro;
        }
        public async Task<TryException<int>> AtualizarCienciaStatus(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, CancellationToken cancellationToken)
        {
            var resultStatus = await _db.Connection.QueryFirstOrDefaultAsync<int>(
                                new CommandDefinition(
                                    commandText: $@"UPDATE 
                                                 tb_dfisoc_solicitacao_ciencia_fi1548
                                                 SET 
                                                 dfisci_idt = @statusCiencia,
                                                 dfisoc_dat_atualizacao = GETDATE()
                                                 WHERE 
                                                 dfisoc_idt = @IdCiencia",
                                    parameters: new
                                    {
                                        IdCiencia = documentoFI1548CienciaModel.CienciaId,
                                        statusCiencia = documentoFI1548CienciaModel.IdStatusCiencia
                                    },
                                    cancellationToken: cancellationToken,
                                    transaction: Transaction
                                )
                            );

            return resultStatus;
        }

        private async Task<TryException<int>> InserirAprovadoresFI1548(int documentoFI1548CienciaId, CancellationToken cancellationToken)
        {
            var listaGuidAprovadoresFI1548 = await ListarGuidDeAprovadoresFI1548(cancellationToken);

            if (listaGuidAprovadoresFI1548.IsFailure)
                return listaGuidAprovadoresFI1548.Failure;

            var listaGuidAprovadores = listaGuidAprovadoresFI1548.Success;

            var query = $@"
                         insert into tb_dfiscua_solicitacao_ciencia_usuario_aprovacao_fi1548
                            (
                             dfisoc_idt
                            ,dfiscua_usu_guid_ad
                            ,dfiscua_flg_ativo
                            ,dfiscua_dat_criacao
                            ,dfiscua_dat_atualizacao
                            )
                         values 
                            (
                             {documentoFI1548CienciaId}
                            ,@guidUsuario
                            ,1
                            ,getdate()
                            ,getdate()
                            );";

            return await _db.Connection.ExecuteAsync(new CommandDefinition(
                    commandText: query,
                    parameters: listaGuidAprovadores,
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                ));
        }

        private async Task<TryException<List<dynamic>>> ListarGuidDeAprovadoresFI1548(CancellationToken cancellationToken)
        {
            var listaGuidAprovadores = new List<dynamic>();

            var result = await _db.Connection.QueryAsync<Guid>(
                    new CommandDefinition(
                        commandText: $@"select distinct tpadpu.tpadpu_guid_ad
                                        from 
	                                        tb_tpad_template_processo_assinatura_documento tpad 
                                            inner join tb_tpadp_template_processo_assinatura_documento_passo tpadp on tpad.tpad_idt = tpadp.tpad_idt
											inner join tb_tpadpu_template_processo_assinatura_documento_passo_usuario tpadpu on tpadp.tpadp_idt = tpadpu.tpadp_idt
                                        where 
	                                        tpad.tpad_nome = @nomeTemplate and
	                                        tpad.tpad_flg_ativo = 1 and
	                                        tpadp.tpadp_flg_ativo = 1 and
	                                        tpadpu.tpadpu_flg_ativo = 1 ",
                        parameters: new
                        {
                            nomeTemplate = "DIR-MESMO-PASSO"
                        },
                        transaction: Transaction,
                        cancellationToken: cancellationToken
                    )
                );


            if (!result.Any())
                return new BusinessException("usuario-aprovador-nao-encontrado", $"Não foi encontrado nenhum aprovador.");

            foreach (var idUsuario in result)
            {
                listaGuidAprovadores.Add(new { guidUsuario = idUsuario });
            }

            return listaGuidAprovadores;
        }

        public async Task<TryException<SoclicitacaoCienciaAprovadoresModel>> ObterAprovadoresCienciaCancelamento(int documentoId, CancellationToken cancellationToken)
        {
            var listaSocCiencia = new Dictionary<int, SoclicitacaoCienciaAprovadoresModel>();
            var result = (await _db.Connection.QueryAsync<SoclicitacaoCienciaAprovadoresModel, CienciaUsuarioAprovacaoModel, SoclicitacaoCienciaAprovadoresModel>(
                   new CommandDefinition(
                       commandText: $@"	 SELECT 
						                      soc.dfisoc_idt [IdSolicitacaoCiencia],
						                      soc.dfisoc_dat_atualizacao [DataAtualizacao],
						                      soc.dfisci_idt [StatusCiencia],

					                          scua.dfiscua_idt [Id],
                                              scua.dfisoc_idt [SolicitacaoCienciaId],
					                          scua.dfiscua_usu_guid_ad [UsuarioGuid],
						                      scua.dfiscua_des_observacao [Observacao],
						                      scua.dfiscua_dat_aprovacao [DataAprovacao],
						                      scua.dfiscua_flg_rejeitado [FlgRejeitado],
						                      scua.dfiscua_dat_atualizacao [DataAtualizacao]
				                       FROM 
			                                 tb_dfisoc_solicitacao_ciencia_fi1548  soc
						                     INNER JOIN  tb_dfiscua_solicitacao_ciencia_usuario_aprovacao_fi1548 scua on scua.dfisoc_idt = soc.dfisoc_idt
				                       WHERE 
						                     soc.dfi_idt = @documentoId
											 and scua.dfiscua_flg_rejeitado = 1
											 and soc.dfitci_idt = @tipoCiencia
                                             and soc.dfisci_idt IN (select convert(int, isnull(
                                                                  (SELECT top 1 dfisci_idt 
                                                                   FROM
                                                                    tb_dfisoc_solicitacao_ciencia_fi1548 
                                                                   WHERE 
                                                                    dfi_idt = @documentoId
                                                                    and dfitci_idt = @tipoCiencia 
                                                                    and dfisci_idt = @statusCiencia 
                                                                   order by 
                                                                    dfisoc_dat_atualizacao desc),@statusCienciaRejeitado)))",
                                   parameters: new
                                   {
                                       documentoId,
                                       tipoCiencia = TipoCienciaFi1548.Cancelamento,
                                       statusCiencia = StatusCienciaFi1548.Concluido,
                                       statusCienciaRejeitado = StatusCienciaFi1548.Rejeitado,
                                   },
                                   cancellationToken: cancellationToken
                   ),
                    splitOn: "IdSolicitacaoCiencia,Id",
                    map: (passoItemMap, cienciaAprovarMap) =>
                    {
                        if (!listaSocCiencia.TryGetValue(passoItemMap.IdSolicitacaoCiencia, out var socCiencia))
                        {
                            socCiencia = passoItemMap;
                            listaSocCiencia.Add(socCiencia.IdSolicitacaoCiencia, socCiencia);
                        }

                        if (cienciaAprovarMap.SolicitacaoCienciaId == socCiencia.IdSolicitacaoCiencia)
                            socCiencia.AddCienciaUsuario(cienciaAprovarMap);

                        return socCiencia;
                    }
                )).GroupBy(x => x.IdSolicitacaoCiencia).Select(x => x.First()).ToCollection();

            return result.OrderByDescending(x => x.DataAtualizacao).FirstOrDefault();
        }

        public async Task<TryException<CienciaUsuariosProvacao>> ObterAprovadoresPorUsuarioECiencia(int idSolicitacaoCiencia, Guid usuarioId, CancellationToken cancellationToken)
        {

            var result = await _db.Connection.QueryAsync<CienciaUsuariosProvacao>(
                    new CommandDefinition(
                        commandText: $@"SELECT scua.dfiscua_idt as [Id],
	                                           scua.dfiscua_usu_guid_ad as [UsuarioId],
	                                           scua.dfiscua_dat_aprovacao as [Aprovacao],
	                                           scua.dfiscua_des_observacao as [Observacao],
	                                           scua.dfiscua_flg_ativo as [Ativo]
	                                           FROM  
                                               tb_dfiscua_solicitacao_ciencia_usuario_aprovacao_fi1548 scua  
	                                           WHERE
											   scua.dfisoc_idt = @idSolicitacaoCiencia and
                                               scua.dfiscua_usu_guid_ad = @usuarioId and
											   scua.dfiscua_dat_aprovacao is null and 
                                               scua.dfiscua_flg_ativo = 1",
                        parameters: new
                        {
                            idSolicitacaoCiencia,
                            usuarioId
                        },
                        cancellationToken: cancellationToken,
                        transaction: Transaction
                    ));

            return result.FirstOrDefault();
        }

        public async Task<TryException<IEnumerable<ObterMotivoCancelamentoModel>>> ObterMotivoCancelamento(int documentoId,  CancellationToken cancellationToken)
        {

            var result = await _db.Connection.QueryAsync<ObterMotivoCancelamentoModel>(
                    new CommandDefinition(
                        commandText: $@"SELECT 

                                            dfisoc.dfisci_idt as Id,
                                            dfisoc.dfisoc_usu_guid_ad as UsuarioGuid,
                                            dfisoc.dfisoc_des_observacao as Observacao,
                                            dfisoc.dfisoc_dat_criacao as DataCancelamento,
                                            'Cancelado' as TipoCancelamento
                                        FROM tb_dfi_documento_fi1548 dfi 
                                            INNER JOIN tb_dfisoc_solicitacao_ciencia_fi1548 dfisoc on dfisoc.dfi_idt = dfi.dfi_idt
                                        WHERE 
                                            dfi.dfi_idt = @documentoId and dfisoc.dfitci_idt = 1 and dfisoc.dfisoc_flg_ativo = 1",
                        parameters: new
                        {
                            documentoId
                        },
                        cancellationToken: cancellationToken,
                        transaction: Transaction
                    ));

            return result.ToCollection();
        }

        public async Task<TryException<IEnumerable<ObterMotivoCancelamentoModel>>> ObterJustificativaReprovacao(int documentoId, CancellationToken cancellationToken)
        {

            var result = await _db.Connection.QueryAsync<ObterMotivoCancelamentoModel>(
                    new CommandDefinition(
                        commandText: $@"SELECT

		                                     padpu.padpu_idt as Id,
                                             padpu.padpu_guid_ad as UsuarioGuid,
                                             padpu.padpu_justificativa as Observacao,
                                             padpu.padpu_dat_criacao as DataCancelamento,
                                             'Reprovado' as TipoCancelamento
                                       FROM tb_pado_processo_assinatura_documento_origem pado 
                                             INNER JOIN tb_padp_processo_assinatura_documento_passo padp on padp.pad_idt = pado.pad_idt
                                             INNER JOIN tb_padpu_processo_assinatura_documento_passo_usuario padpu on padpu.padp_idt = padp.padp_idt
                                       WHERE
                                             pado.pado_origem_idt = @documentoId and padpu.padpu_flg_ativo = 1 and padpu.sadpu_idt = 4",
                        parameters: new
                        {
                            documentoId
                        },
                        cancellationToken: cancellationToken,
                        transaction: Transaction
                    ));

            return result.ToCollection();
        }

        public async Task<TryException<IEnumerable<CienciaUsuariosProvacao>>> ObterAprovadoresPorCiencia(int idSolicitacaoCiencia, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<CienciaUsuariosProvacao>(
                    new CommandDefinition(
                        commandText: $@"SELECT scua.dfiscua_idt as [Id],
	                                           scua.dfiscua_usu_guid_ad as [UsuarioId],
	                                           scua.dfiscua_dat_aprovacao as [Aprovacao],
	                                           scua.dfiscua_des_observacao as [Observacao],
	                                           scua.dfiscua_flg_ativo as [Ativo],
	                                           scua.dfiscua_flg_rejeitado as [FlgRejeitado]
	                                           FROM  
                                               tb_dfiscua_solicitacao_ciencia_usuario_aprovacao_fi1548 scua  
	                                           WHERE
                                               scua.dfisoc_idt = @idSolicitacaoCiencia and
	                                           scua.dfiscua_flg_ativo = 1",
                        parameters: new
                        {
                            idSolicitacaoCiencia
                        },
                        cancellationToken: cancellationToken,
                        transaction: Transaction
                    ));

            return result.ToList();
        }

        public async Task<TryException<Return>> AtualizarStatus(int documentoFI1548Id, DocumentoFI1548Status novoStatus, CancellationToken cancellationToken)
        {
            await _db.Connection.ExecuteAsync(
                new CommandDefinition(
                    commandText: @"
                        UPDATE 
                            [dbo].[tb_dfi_documento_fi1548]
                        SET 
                            sdo_idt = @StatusId,
                            dfi_dat_atualizacao = GETDATE()
                        WHERE 
                            dfi_idt = @Id",
                    transaction: Transaction,
                    cancellationToken: cancellationToken,
                    parameters: new
                    {
                        Id = documentoFI1548Id,
                        StatusId = novoStatus.ToInt32()
                    }
                )
            );

            return Return.Empty;
        }
        public async Task<TryException<DocumentoFI1548Model>> ObterStatusPagamentoPorId(int documentoId, CancellationToken cancellationToken)
        {

            var result = await _db.Connection.QueryAsync<DocumentoFI1548Model>(
                    new CommandDefinition(
                        commandText: $@"SELECT

                                            dfi_idt as Id,
                                            dfi_descricao as Descricao,
                                            dfi_referencia as Referencia,
                                            dfi_valor_total as Valor,
                                            dfi_pagamento as TipoPagamento,
                                            dfi_quantidade_parcelas as QuantidadeParcelas,
                                            sdo_idt as Status,
                                            dfi_guid_usuario_ad as AutorId,
                                            dfi_numero as Numero,
                                            dfi_dat_criacao as DataCriacao,
                                            dfi_dat_atualizacao as DataAtualizacao,
                                            dfi_dat_liquidacao as DataLiquidacao,
                                            dfi_flg_ativo as Ativo,
                                            dfi_moeda_simbolo as MoedaSimbolo,
                                            dfi_referencia_substituto as ReferenciaSubstituto

                                         from tb_dfi_documento_fi1548 where dfi_idt = @documentoId ",
                        parameters: new
                        {
                            documentoId
             
                        },
                        cancellationToken: cancellationToken,
                        transaction: Transaction
                    ));

            return result.FirstOrDefault();
        }

    }
}
