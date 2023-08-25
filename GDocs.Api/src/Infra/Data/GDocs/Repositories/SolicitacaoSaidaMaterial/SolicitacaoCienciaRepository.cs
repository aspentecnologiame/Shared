using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using Dapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoCiencia;
using Microsoft.Extensions.Configuration;
using System.Linq;
using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories.SolicitacaoCiencia
{
    internal class SolicitacaoCienciaRepository : Repository, ISolicitacaoCienciaRepository
    {
        private readonly IConfiguration _configuration;

        public SolicitacaoCienciaRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork,
            IConfiguration configuration) : base(db, unitOfWork)
        {
            _configuration = configuration;
        }

        public async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPeloUsuario(Guid activeDirectoryId, CancellationToken cancellationToken)
        {
            var processoCiencia = _configuration.GetValue("DocumentoFI347:ProcessoAssinaturaDocumentoOrigemNome", string.Empty);

            var result = await _db.Connection.QueryAsync<ProcessoAssinaturaDocumentoModel>(
                    new CommandDefinition(
                        commandText: $@"SELECT 
	                                        soc.soc_idt as [CienciaId],
                                            soc.soc_des_observacao AS [Observacao],
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
																when aux.scua_flg_rejeitado = 0 then  2
																when aux.scua_flg_rejeitado = 1 then  4
															 end 
	                                              from 
	                                                tb_scua_solicitacao_ciencia_usuario_aprovacao aux 
	                                              where 
	                                                aux.soc_idt = soc.soc_idt and
		                                            aux.scua_usu_guid_ad != scua.scua_usu_guid_ad and
		                                            aux.scua_dat_aprovacao is not null and
		                                            aux.scua_flg_ativo = 1
	                                            ), 1)) AS [StatusPassoAtualId]
                                        FROM 
	                                        tb_soc_solicitacao_ciencia soc INNER JOIN  tb_scua_solicitacao_ciencia_usuario_aprovacao scua
								                                            on soc.soc_idt = scua.soc_idt and
									                                           scua.scua_flg_ativo = 1
								                                           INNER JOIN tb_pado_processo_assinatura_documento_origem pado
								                                            on pado.pado_origem_idt = soc.ssm_idt and
									                                           pado_origem_nome = @processoCiencia
								                                           INNER JOIN tb_pad_processo_assinatura_documento pad
								                                            on pad.pad_idt = pado.pad_idt
								                                           INNER JOIN tb_padc_processo_assinatura_documento_categoria padc
								                                            on padc.padc_idt = pad.padc_idt
                                        WHERE
	                                        soc.soc_flg_ativo = 1 and
                                            soc.sci_idt = 1 and
	                                        scua.scua_usu_guid_ad = @activeDirectoryId and
	                                        scua.scua_dat_aprovacao is null
                                        ORDER BY 
                                            pad.pad_numero_documento",
                        parameters: new
                        {
                            activeDirectoryId,
                            processoCiencia
                        },
                        cancellationToken: cancellationToken
                    )
                );

            return result.ToList();
        }

        public async Task<TryException<SolicitacaoCienciaModel>> ObterCienciaPeloIdentificador(int idSolicitacaoCiencia, CancellationToken cancellationToken)
        {
            var listaCiencia = new Dictionary<int, SolicitacaoCienciaModel>();

            var listaSmataIdt = new List<int>();
            listaSmataIdt.Add(SaidaMaterialTipoAcao.SolicitacaoProrrogacao.ToInt32());
            listaSmataIdt.Add(SaidaMaterialTipoAcao.SolicitacaoCancelamento.ToInt32());
            listaSmataIdt.Add(SaidaMaterialTipoAcao.SolicitacaoBaixaSemRetorno.ToInt32());
            
            var result = await _db.Connection.QueryAsync<SolicitacaoCienciaModel, SolicitacaoCienciaItemModel, SolicitacaoCienciaModel>(
                    new CommandDefinition(
                        commandText: $@"SELECT 
                                            soc.soc_idt as [Id],
                                            soc.ssma_idt as [IdSolicitacaoSaidaMaterialAcao],
                                            soc.ssm_idt as [IdSolicitacaoSaidaMaterial],
                                            soc.tci_idt as [IdTipoCiencia],
                                            tci.tci_des as [TipoCiencia],
                                            soc.sci_idt as [IdStatusCiencia],
                                            sci.sci_des as [StatusCiencia],
                                            soc.soc_dat_prorrogacao as [DataProrrogacao],
                                            ssm.ssm_guid_ad_responsavel as [IdUsuario],
                                            soc.soc_flg_ativo as [FlgAtivo],
                                            ssm.ssm_num as [NumeroMaterial],
                                            ssm.ssm_des_motivo as [MotivoSolicitacao],
                                            ssm.ssm_dat_retorno as [DataRetorno],
										    (SELECT TOP 1 ssma_des_observacao 
                                            FROM [dbo].[tb_ssma_solicitacao_saida_material_acao] ssma
                                            WHERE 
                                            ssma.ssm_idt = ssm.ssm_idt AND ssma.ssma_des_observacao IS NOT NULL AND
                                            ssma.smta_idt in @listaSmataIdt
                                            ORDER BY
                                            ssma.ssma_dat_atualizacao 
                                            DESC)
                                            AS Justificativa,

	                                        scit.scit_idt as [IdItem],
	                                        scit.ssmi_idt as [IdSolicitacaoSaidaMaterialItem],
	                                        ssmi.ssmi_qtd_item as [Quantidade],
	                                        ssmi.ssmi_unidade as [Unidade],
	                                        ssmi.ssm_num_patrimonio as [Patrimonio],
	                                        ssmi.ssmi_descricao as [Descricao]
                                        FROM 
	                                        tb_soc_solicitacao_ciencia soc INNER JOIN tb_tci_tipo_ciencia tci
                                                                              on soc.tci_idt = tci.tci_idt
                                                                           INNER JOIN tb_sci_status_ciencia sci
                                                                              on soc.sci_idt = sci.sci_idt
	                                                                       INNER JOIN tb_scit_solicitacao_ciencia_item scit
                                                                              on soc.soc_idt = scit.soc_idt
                                                                           INNER JOIN tb_ssmi_solicitacao_saida_material_item ssmi
											                                  on scit.ssmi_idt = ssmi.ssmi_idt
                                                                           INNER JOIN tb_ssm_solicitacao_saida_material ssm 
                                                                              on soc.ssm_idt = ssm.ssm_idt
                                        WHERE
                                            soc.soc_idt = @idSolicitacaoCiencia
	                                    and soc.soc_flg_ativo = 1
	                                    and scit.scit_flg_ativo = 1
	                                    and ssmi.ssmi_flg_ativo = 1",
                        parameters: new
                        {
                            listaSmataIdt,
                            idSolicitacaoCiencia
                        },
                        cancellationToken: cancellationToken
                    ),
                    splitOn: "Id,IdItem",
                    map: (cienciaMap, itemMap) =>
                    {
                        if (!listaCiencia.TryGetValue(cienciaMap.Id, out var ciencia))
                        {
                            ciencia = cienciaMap;
                            listaCiencia.Add(ciencia.Id, ciencia);
                        }

                        ciencia.Itens.Add(itemMap);

                        return ciencia;
                    }
                );

            return result.FirstOrDefault();
        }

        public async Task<TryException<IEnumerable<CienciaUsuariosProvacao>>> ObterAprovadoresPorCiencia(int idSolicitacaoCiencia, CancellationToken cancellationToken)
        {

            var result = await _db.Connection.QueryAsync<CienciaUsuariosProvacao>(
                    new CommandDefinition(
                        commandText: $@"SELECT scua.scua_idt as [Id],
	                                           scua.scua_usu_guid_ad as [UsuarioId],
	                                           scua.scua_dat_aprovacao as [Aprovacao],
	                                           scua.scua_des_observacao as [Observacao],
	                                           scua.scua_flg_ativo as [Ativo],
	                                           scua.scua_flg_rejeitado as [FlgRejeitado]
	                                           FROM  
                                               tb_scua_solicitacao_ciencia_usuario_aprovacao scua  

	                                           WHERE
                                               scua.soc_idt = @idSolicitacaoCiencia and
	                                           scua.scua_flg_ativo = 1",
                        parameters: new
                        {
                            idSolicitacaoCiencia
                        },
                        cancellationToken: cancellationToken,
                        transaction: Transaction
                    ));

            return result.ToList();
        }


        public async Task<TryException<CienciaUsuariosProvacao>> ObterAprovadoresPorUsuarioECiencia(int idSolicitacaoCiencia, Guid usuarioId, CancellationToken cancellationToken)
        {

            var result = await _db.Connection.QueryAsync<CienciaUsuariosProvacao>(
                    new CommandDefinition(
                        commandText: $@"SELECT scua.scua_idt as [Id],
	                                           scua.scua_usu_guid_ad as [UsuarioId],
	                                           scua.scua_dat_aprovacao as [Aprovacao],
	                                           scua.scua_des_observacao as [Observacao],
	                                           scua.scua_flg_ativo as [Ativo]
	                                           FROM  
                                               tb_scua_solicitacao_ciencia_usuario_aprovacao scua  

	                                           WHERE
											   scua.soc_idt = @idSolicitacaoCiencia and
                                               scua.scua_usu_guid_ad = @usuarioId and
											   scua.scua_dat_aprovacao is null and 
                                               scua.scua_flg_ativo = 1",
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


        public async Task<TryException<int>> Inserir(SolicitacaoCienciaModel solicitacaoCiencia, CancellationToken cancellationToken)
        { 
            var idSolicitacaoCiencia = await _db.Connection.QueryFirstAsync<int>(new CommandDefinition(
                    commandText: @"
                         INSERT INTO [dbo].[tb_soc_solicitacao_ciencia]
			                   (
                                ssm_idt,
                                ssma_idt,
                                tci_idt,
                                sci_idt,
                                soc_usu_guid_ad,
                                soc_des_observacao,
                                soc_dat_prorrogacao,
                                soc_flg_ativo,
                                soc_dat_criacao,
                                soc_dat_atualizacao
                               )
		                 VALUES
			                   (
                                @IdSolicitacaoSaidaMaterial
                                ,@IdSolicitacaoSaidaMaterialAcao
			                   ,@IdTipoCiencia
			                   ,@IdStatusCiencia
			                   ,@IdUsuario
			                   ,@Observacao
			                   ,@DataProrrogacao
			                   ,@FlgAtivo
			                   ,GETDATE()
			                   ,GETDATE()
                            );

                            SELECT CAST(SCOPE_IDENTITY() as [int]);",
                    parameters: solicitacaoCiencia,
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                ));

            if (solicitacaoCiencia.Itens.Any())
			{
                await InserirItens(
                    idSolicitacaoCiencia,
                    solicitacaoCiencia.Itens,
                    cancellationToken);
            }

            var aprovadores = await InserirAprovadores(idSolicitacaoCiencia, cancellationToken);

            if (aprovadores.IsFailure)
                return aprovadores.Failure;

            return idSolicitacaoCiencia;
        }

        public async Task<TryException<int?>> ObterTipoDeCienciaAtravesDoTipoDeAcao(int idTipoAcao, CancellationToken cancellationToken)
        {
                var result = await _db.Connection.QueryFirstOrDefaultAsync<int?>(
                                    new CommandDefinition(
                                        commandText: $@"select 
	                                        tci_idt 
                                        from 
	                                        tb_smta_saida_material_tipo_acao
                                        where
	                                        smta_idt = @idTipoAcao",
                                        parameters: new
                                        {
                                            idTipoAcao
                                        },
                                        transaction: Transaction,
                                        cancellationToken: cancellationToken
                                    )
                                );

            return result;
        }

        public async Task<TryException<int>> RegistroCiencia(SolicitacaoCienciaModel solicitacaoCiencia, int idCienciaAprovador, CancellationToken cancellationToken)
        {
       
                var result = await _db.Connection.QueryFirstOrDefaultAsync<int>(
                                    new CommandDefinition(
                                        commandText: $@"UPDATE 
                                                 tb_scua_solicitacao_ciencia_usuario_aprovacao 
                                                 SET 
                                                 scua_dat_aprovacao = GETDATE(),
                                                 scua_flg_rejeitado = @flagRejeitar,
                                                 scua_des_observacao = @observacao
                                                 WHERE 
                                                 scua_idt = @IdAprovacao",
                                        parameters: new
                                        {
                                            IdAprovacao = idCienciaAprovador,
                                            flagRejeitar = solicitacaoCiencia.Ciente,
                                            observacao = solicitacaoCiencia.Observacao,

                                        },
                                        cancellationToken: cancellationToken,
                                        transaction: Transaction
                                    )
                                );

            return result;
        }
        public async Task<TryException<int>> AtualizarCienciaStatus(SolicitacaoCienciaModel solicitacaoCiencia, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryFirstOrDefaultAsync<int>(
                                new CommandDefinition(
                                    commandText: $@"UPDATE 
                                                 tb_soc_solicitacao_ciencia 
                                                 SET 
                                                 sci_idt = @statusCiencia,
                                                 soc_dat_atualizacao = GETDATE()
                                                 WHERE 
                                                 soc_idt = @IdCiencia",
                                    parameters: new
                                    {
                                        IdCiencia = solicitacaoCiencia.Id,
                                        statusCiencia = solicitacaoCiencia.IdStatusCiencia
                                    },
                                    cancellationToken: cancellationToken,
                                    transaction: Transaction
                                )
                            );

            return result;
        }

        private async Task<TryException<int>> InserirItens(int idSolicitacaoCiencia, List<SolicitacaoCienciaItemModel> ListaSolicitacaoCienciaItem, CancellationToken cancellationToken)
        {
            var listaIdSolicitacaoSaidaMaterialItem = new List<dynamic>();
            ListaSolicitacaoCienciaItem.ForEach(item => listaIdSolicitacaoSaidaMaterialItem.Add(new { idItem = item.IdSolicitacaoSaidaMaterialItem }));

            var query = $@"
                         INSERT INTO [dbo].[tb_scit_solicitacao_ciencia_item]
			                   (
                                 soc_idt
                                ,ssmi_idt
                                ,scit_flg_ativo
                                ,scit_dat_criacao
                                ,scit_dat_atualizacao
                               )
		                 VALUES
			                   (
                                {idSolicitacaoCiencia}
			                   ,@idItem
			                   ,1
			                   ,GETDATE()
			                   ,GETDATE()
                            );";

			return await _db.Connection.ExecuteAsync(new CommandDefinition(
					commandText: query,
					parameters: listaIdSolicitacaoSaidaMaterialItem,
					transaction: Transaction,
					cancellationToken: cancellationToken
				));
		}

        private async Task<TryException<int>> InserirAprovadores(int idSolicitacaoCiencia, CancellationToken cancellationToken)
        {
            var listaGuidAprovadores = await ListarGuidDeAprovadores(cancellationToken);

            if (listaGuidAprovadores.IsFailure)
                return listaGuidAprovadores.Failure;

            var query = $@"
                         insert into tb_scua_solicitacao_ciencia_usuario_aprovacao
                            (
                             soc_idt
                            ,scua_usu_guid_ad
                            ,scua_flg_ativo
                            ,scua_dat_criacao
                            ,scua_dat_atualizacao
                            )
                         values 
                            (
                             {idSolicitacaoCiencia}
                            ,@guidUsuario
                            ,1
                            ,getdate()
                            ,getdate()
                            );";

            return await _db.Connection.ExecuteAsync(new CommandDefinition(
                    commandText: query,
                    parameters: listaGuidAprovadores.Success,
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                ));
        }

        private async Task<TryException<List<dynamic>>> ListarGuidDeAprovadores(CancellationToken cancellationToken)
        {
            var listaGuidAprovadores = new List<dynamic>();

			var result = await _db.Connection.QueryAsync<Guid>(
					new CommandDefinition(
						commandText: $@"select distinct 
	                                         tpadpu.tpadpu_guid_ad
                                        from 
	                                        tb_tpad_template_processo_assinatura_documento tpad inner join tb_tpadp_template_processo_assinatura_documento_passo tpadp
														                                           on tpad.tpad_idt = tpadp.tpad_idt
														                                        inner join tb_tpadpu_template_processo_assinatura_documento_passo_usuario tpadpu
														                                           on tpadp.tpadp_idt = tpadpu.tpadp_idt

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

        public async Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarObsUsuarioPorCiencia(int IdSolicitacaoCiencia, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<CienciaUsuarioAprovacaoModel>(
                    new CommandDefinition(
                        commandText: $@"
		                    	  SELECT
                                     u.scua_idt as Id
                                    ,u.scua_usu_guid_ad as UsuarioGuid
                                    ,u.scua_des_observacao as Observacao
                                    ,u.scua_dat_aprovacao as DataAprovacao
                                    ,u.scua_flg_rejeitado as FlgRejeitado
                                 FROM 
                                    tb_scua_solicitacao_ciencia_usuario_aprovacao u
                                 WHERE
                                    u.scua_flg_rejeitado = 1
                                    AND u.soc_idt = @IdSolicitacaoCiencia ",
                        parameters: new
                        {
                            IdSolicitacaoCiencia,
                        },
                        cancellationToken: cancellationToken
                    )
                );

            return result.ToCollection();
        }

        public async Task<TryException<SoclicitacaoCienciaAprovadoresModel>> ObterAprovadoresCienciaCancelamento(int solicitacaoSaidaMaterialId, CancellationToken cancellationToken)
        {
            var listaSoc = new Dictionary<int, SoclicitacaoCienciaAprovadoresModel>();
            var result = (await _db.Connection.QueryAsync<SoclicitacaoCienciaAprovadoresModel, CienciaUsuarioAprovacaoModel, SoclicitacaoCienciaAprovadoresModel>(
                   new CommandDefinition(
                       commandText: $@"SELECT 
						                      soc.soc_idt [IdSolicitacaoCiencia],
						                      soc.soc_dat_atualizacao [DataAtualizacao],
						                      soc.sci_idt [StatusCiencia],

					                          scua.scua_idt [Id],
                                              scua.soc_idt [SolicitacaoCienciaId],
					                          scua.scua_usu_guid_ad [UsuarioGuid],
						                      scua.scua_des_observacao [Observacao],
						                      scua.scua_dat_aprovacao [DataAprovacao],
						                      scua.scua_flg_rejeitado [FlgRejeitado],
						                      scua.scua_dat_atualizacao [DataAtualizacao]
				                       FROM 
			                                 tb_soc_solicitacao_ciencia  soc
						                     INNER JOIN  tb_scua_solicitacao_ciencia_usuario_aprovacao scua on scua.soc_idt = soc.soc_idt
				                       WHERE 
						                     soc.ssm_idt = @solicitacaoSaidaMaterialId
											 and scua.scua_flg_rejeitado = 1
											 and soc.tci_idt = @tipoCiencia
                                             and soc.sci_idt IN (select convert(int, isnull(
                                                                  (SELECT top 1 sci_idt 
                                                                   FROM
                                                                    tb_soc_solicitacao_ciencia 
                                                                   WHERE 
                                                                    ssm_idt = @solicitacaoSaidaMaterialId 
                                                                    and tci_idt = @tipoCiencia 
                                                                    and sci_idt = @statusCiencia 
                                                                   order by 
                                                                    soc_dat_atualizacao desc),@tipoCiencia)))",
                                   parameters: new { 
                                       solicitacaoSaidaMaterialId,
                                       tipoCiencia = TipoCiencia.Cancelamento,
                                       statusCiencia = StatusCiencia.Concluido
                                   },
                                   cancellationToken: cancellationToken
                   ),
                    splitOn: "IdSolicitacaoCiencia,Id",
                    map: (passoItemMap, cienciaAprovarMap) =>
                    {
                        if (!listaSoc.TryGetValue(passoItemMap.IdSolicitacaoCiencia, out var socCiencia)){
                            socCiencia = passoItemMap;
                            listaSoc.Add(socCiencia.IdSolicitacaoCiencia, socCiencia);
                        }

                        if (cienciaAprovarMap.SolicitacaoCienciaId == socCiencia.IdSolicitacaoCiencia)
                            socCiencia.AddCienciaUsuario(cienciaAprovarMap);
                       
                        return socCiencia;
                    }
                )).GroupBy(x => x.IdSolicitacaoCiencia).Select(x => x.First()).ToCollection();

            return result.OrderByDescending(x => x.DataAtualizacao).FirstOrDefault();
        }
    }
}
