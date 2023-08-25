using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.Data.Core.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Dapper;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Common.Core.Exceptions;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories.SaidaMaterialNotaFiscal.Service
{
    internal class SaidaMaterialNotaFiscalCienciaRepository : Repository, ISaidaMaterialNotaFiscalCienciaRepository
    {
        private readonly IConfiguration _config;
        public SaidaMaterialNotaFiscalCienciaRepository(IGDocsDatabase db, IUnitOfWork unitOfWork, IConfiguration config) : base(db, unitOfWork)
        {
            _config = config;
        }

        public async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPeloUsuario(Guid activeDirectoryId, CancellationToken cancellationToken)
        {
            var processoCienciaNF = _config.GetValue("SaidaMaterialNotaFiscal:ProcessoAssinaturaNomeDocumento", string.Empty);
            var categoria = _config.GetValue("SaidaMaterialNotaFiscal:Categoria", string.Empty);


            var resultNF = await _db.Connection.QueryAsync<ProcessoAssinaturaDocumentoModel>(
                    new CommandDefinition(
                        commandText: $@"
                                    SELECT  
                                        socnf.socnf_idt AS [CienciaId], 
                                        socnf.socnf_des_observacao as [Observacao],
                                        0 AS [Id],
                                        '{processoCienciaNF}' AS [Titulo],
                                        smnf.smnf_motivo AS [Descricao], 
                                        '{processoCienciaNF}' AS [NomeDocumento],
                                        0 AS [Destaque],
                                        smnf.smnf_guid_ad_autor AS [AutorId],
                                        3 AS [StatusId],
                                        smnf.smnf_dat_criacao AS [DataCriacao],
                                        smnf.smnf_num AS [Numero],
                                        '{categoria}' AS CategoriaId,
                                        '{processoCienciaNF}' AS DescricaoCategoria,
                                        null AS [AssinarCertificadoDigital], 
                                        null AS  [AssinarFisicamente],
                                        null AS [AssinadoPorMimVisivel],
                                        1 AS [UsuarioDaConsultaAssinou],
						                convert(int, isnull((select top 1
                                                             case 
																when aux.scunfa_flg_rejeitado = 0 then  2
																when aux.scunfa_flg_rejeitado = 1 then  4
															 end 
	                                                          from 
	                                                           tb_scunfa_solicitacao_ciencia_usuario_nota_fiscal_aprovacao aux 
	                                                          where 
	                                                            aux.socnf_idt = socnf.socnf_idt and
		                                                        aux.scunfa_usu_guid_ad != scunfa.scunfa_usu_guid_ad and
		                                                        aux.scunfa_dat_aprovacao is not null and
		                                                        aux.scunfa_flg_ativo = 1 ), 1)) AS [StatusPassoAtualId]
                                    FROM
                                        tb_socnf_solicitacao_ciencia_nota_fiscal socnf
                                        inner join tb_scunfa_solicitacao_ciencia_usuario_nota_fiscal_aprovacao scunfa  on scunfa.socnf_idt = socnf.socnf_idt
                                        inner join tb_smnf_saida_material_nota_fiscal smnf on smnf.smnf_idt = socnf.smnf_idt
                                    WHERE 
                                        scunfa.scunfa_usu_guid_ad = @activeDirectoryId  
                                        AND scunfa.scunfa_dat_aprovacao  is null
	                                ORDER BY 
                                        socnf.socnf_idt  ASC",
                        parameters: new
                        {
                            activeDirectoryId,
                        },
                        cancellationToken: cancellationToken
                    )
                );

            return resultNF.ToList();
        }

        public async Task<TryException<SaidaMaterialNotaFiscalCienciaModel>> ObterCienciaPeloIdentificador(int idSaidaMaterialNotaFiscalCiencia, CancellationToken cancellationToken)
        {
            var listaCienciaNF = new Dictionary<int, SaidaMaterialNotaFiscalCienciaModel>();

            var listaSmataNFIdt = new List<int>();
            listaSmataNFIdt.Add(SaidaMaterialTipoAcao.SolicitacaoProrrogacao.ToInt32());
            listaSmataNFIdt.Add(SaidaMaterialTipoAcao.SolicitacaoBaixaSemRetorno.ToInt32());


            var result = await _db.Connection.QueryAsync<SaidaMaterialNotaFiscalCienciaModel, SaidaMaterialNotaFiscalCienciaItemModel, SaidaMaterialNotaFiscalCienciaModel>(
                    new CommandDefinition(
                        commandText: $@"SELECT 
                                            socnf.socnf_idt as [Id],
                                            socnf.smnfa_idt as [IdSaidaMaterialNotaFiscalAcao],
                                            socnf.smnf_idt as  [IdSaidaMaterialNotaFiscal],
                                            socnf.tcinf_idt as [IdTipoCiencia],
                                            tcinf.tcinf_des as [TipoCiencia],
                                            socnf.scinf_idt as [IdStatusCiencia],
                                            scinf.scinf_des as [StatusCiencia],  
                                            socnf.socnf_dat_prorrogacao as [DataProrrogacaoNF],
                                            smnf.smnf_guid_ad_autor as [IdUsuario],
                                            socnf.socnf_des_observacao as [Observacao],
                                            socnf.socnf_flg_ativo as [FlgAtivo],
                                            smnf.smnf_num as [NumeroMaterial],
                                            smnf.smnf_motivo [MotivoSolicitacao],
                                            smnf.smnf_dat_retorno as [DataRetorno],
										    (SELECT TOP 1 smnfa.smnfa_des_observacao 
                                            FROM [dbo].[tb_smnfa_saida_material_nota_fiscal_acao] smnfa
                                            WHERE 
                                            smnfa.smnf_idt = smnf.smnf_idt AND smnfa.smnfa_des_observacao IS NOT NULL AND
                                            smnfa.smtanf_idt in @listaSmataNFIdt
                                            ORDER BY
                                            smnfa.smnfa_dat_atualizacao 
                                            DESC)
                                            AS Justificativa,

	                                        scitnf.scitnf_idt as [IdItem],
	                                        scitnf.smnfi_idt as [IdSaidaMaterialNotaFiscalItem], 
	                                        smnfi.smnfi_qtd as [Quantidade],
	                                        smnfi.smnfi_unidade as [Unidade],
	                                        smnfi.smnfi_valor_unitario as [ValorUnitario],
	                                        smnfi.smnfi_tag_servico as [TagService],
	                                        smnfi.smnfi_codigo as [Codigo],
	                                        smnfi.smnfi_patrimonio as [Patrimonio],
	                                        smnfi.smnfi_des as [Descricao]
                                        FROM 
	                                        tb_socnf_solicitacao_ciencia_nota_fiscal socnf INNER JOIN tb_tcinf_tipo_ciencia_nota_fiscal tcinf
                                                                              on socnf.tcinf_idt = tcinf.tcinf_idt
                                                                           INNER JOIN tb_scinf_status_ciencia_nota_fiscal scinf
                                                                              on socnf.scinf_idt = scinf.scinf_idt
	                                                                       INNER JOIN tb_scitnf_solicitacao_ciencia_item_nota_fiscal scitnf
                                                                              on socnf.socnf_idt = scitnf.socnf_idt
                                                                           INNER JOIN tb_smnfi_saida_material_nota_fiscal_item smnfi
											                                  on scitnf.smnfi_idt = smnfi.smnfi_idt
                                                                           INNER JOIN tb_smnf_saida_material_nota_fiscal smnf 
                                                                              on socnf.smnf_idt = smnf.smnf_idt
                                        WHERE
                                        socnf.socnf_idt = @idSaidaMaterialNotaFiscalCiencia
										and socnf.socnf_flg_ativo = 1
	                                    and scitnf.scitnf_flg_ativo = 1
	                                    and smnfi.smnfi_flg_ativo = 1",
                        parameters: new
                        {
                            listaSmataNFIdt,
                            idSaidaMaterialNotaFiscalCiencia
                        },
                        cancellationToken: cancellationToken
                    ),
                    splitOn: "Id,IdItem",
                    map: (cienciaMap, itemMap) =>
                    {
                        if (!listaCienciaNF.TryGetValue(cienciaMap.Id, out var ciencia))
                        {
                            ciencia = cienciaMap;
                            listaCienciaNF.Add(ciencia.Id, ciencia);
                        }

                        ciencia.Itens.Add(itemMap);

                        return ciencia;
                    }
                );

            return result.FirstOrDefault();
        }

        public async Task<TryException<IEnumerable<CienciaUsuariosProvacao>>> ObterAprovadoresPorCiencia(int idSaidaMaterialNotaFiscalCiencia, CancellationToken cancellationToken)
        {
            var resultNF = await _db.Connection.QueryAsync<CienciaUsuariosProvacao>(
                    new CommandDefinition(
                        commandText: $@"SELECT scunfa.scunfa_idt as [Id],
	                                           scunfa.scunfa_usu_guid_ad as [UsuarioId],
	                                           scunfa.scunfa_dat_aprovacao as [Aprovacao],
	                                           scunfa.scunfa_des_observacao as [Observacao],
	                                           scunfa.scunfa_flg_ativo as [Ativo],
	                                           scunfa.scunfa_flg_rejeitado as [FlgRejeitado]
	                                           FROM  
                                               tb_scunfa_solicitacao_ciencia_usuario_nota_fiscal_aprovacao scunfa  
	                                           WHERE
                                               scunfa.socnf_idt = @idSaidaMaterialNotaFiscalCiencia and
	                                           scunfa.scunfa_flg_ativo = 1",
                        parameters: new
                        {
                            idSaidaMaterialNotaFiscalCiencia
                        },
                        cancellationToken: cancellationToken,
                        transaction: Transaction
                    ));

            return resultNF.ToList();
        }

        public async Task<TryException<CienciaUsuariosProvacao>> ObterAprovadoresPorUsuarioECiencia(int idSaidaMaterialNotaFiscalCiencia, Guid usuarioId, CancellationToken cancellationToken)
        {
            var resultNF = await _db.Connection.QueryAsync<CienciaUsuariosProvacao>(
                    new CommandDefinition(
                        commandText: $@"SELECT scunfa.scunfa_idt as [Id],
	                                           scunfa.scunfa_usu_guid_ad as [UsuarioId],
	                                           scunfa.scunfa_dat_aprovacao as [Aprovacao],
	                                           scunfa.scunfa_des_observacao as [Observacao],
	                                           scunfa.scunfa_flg_ativo as [Ativo]
	                                           FROM  
                                               tb_scunfa_solicitacao_ciencia_usuario_nota_fiscal_aprovacao scunfa   
	                                           WHERE
											   scunfa.socnf_idt = @idSaidaMaterialNotaFiscalCiencia and
                                               scunfa.scunfa_usu_guid_ad = @usuarioId and
											   scunfa.scunfa_dat_aprovacao is null and 
                                               scunfa.scunfa_flg_ativo = 1",
                        parameters: new
                        {
                            idSaidaMaterialNotaFiscalCiencia,
                            usuarioId
                        },
                        cancellationToken: cancellationToken,
                        transaction: Transaction
                    ));

            return resultNF.FirstOrDefault();
        }

        public async Task<TryException<int>> Inserir(SaidaMaterialNotaFiscalCienciaModel saidaMaterialNotaFiscalCienciaModel, CancellationToken cancellationToken)
        {
            var idSaidaMaterialNotaFiscalCienciaNF = await _db.Connection.QueryFirstAsync<int>(new CommandDefinition(
                    commandText: @"
                         INSERT INTO [dbo].[tb_socnf_solicitacao_ciencia_nota_fiscal]
			                   (
                                smnf_idt,
                                smnfa_idt,
                                tcinf_idt,
                                scinf_idt,
                                socnf_usu_guid_ad,
                                socnf_des_observacao,
                                socnf_dat_prorrogacao,
                                socnf_flg_ativo,
                                socnf_dat_criacao,
                                socnf_dat_atualizacao
                               )
		                 VALUES
			                   (
                                @IdSaidaMaterialNotaFiscal
                                ,@IdSaidaMaterialNotaFiscalAcao
			                   ,@IdTipoCiencia
			                   ,@IdStatusCiencia
			                   ,@IdUsuario
			                   ,@Observacao
			                   ,@DataProrrogacaoNF
			                   ,@FlgAtivo
			                   ,GETDATE()
			                   ,GETDATE()
                            );

                            SELECT CAST(SCOPE_IDENTITY() as [int]);",
                    parameters: saidaMaterialNotaFiscalCienciaModel,
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                ));

            if (saidaMaterialNotaFiscalCienciaModel.Itens.Any())
            {
                await InserirItens(
                    idSaidaMaterialNotaFiscalCienciaNF,
                    saidaMaterialNotaFiscalCienciaModel.Itens,
                    cancellationToken);
            }

            var aprovadoresNF = await InserirAprovadores(idSaidaMaterialNotaFiscalCienciaNF, cancellationToken);

            if (aprovadoresNF.IsFailure)
                return aprovadoresNF.Failure;

            return idSaidaMaterialNotaFiscalCienciaNF;
        }

        public async Task<TryException<int?>> ObterTipoDeCienciaAtravesDoTipoDeAcao(int idTipoAcao, CancellationToken cancellationToken)
        {
            var resultNF = await _db.Connection.QueryFirstOrDefaultAsync<int?>(
                                    new CommandDefinition(
                                        commandText: $@"select 
	                                        tcinf_idt 
                                        from 
	                                        tb_smtanf_saida_material_tipo_acao_nota_fiscal
                                        where
	                                        smtanf_idt = @idTipoAcao",
                                        parameters: new
                                        {
                                            idTipoAcao
                                        },
                                        transaction: Transaction,
                                        cancellationToken: cancellationToken
                                    )
                                );

            return resultNF;
        }

        public async Task<TryException<int>> RegistroCiencia(SaidaMaterialNotaFiscalCienciaModel saidaMaterialNotaFiscalCienciaModel, int idCienciaAprovador, CancellationToken cancellationToken)
        {
            var resultNF = await _db.Connection.QueryFirstOrDefaultAsync<int>(
                                    new CommandDefinition(
                                        commandText: $@"UPDATE 
                                                 tb_scunfa_solicitacao_ciencia_usuario_nota_fiscal_aprovacao 
                                                 SET 
                                                 scunfa_dat_aprovacao = GETDATE(),
                                                 scunfa_flg_rejeitado = @flagRejeitar,
                                                 scunfa_des_observacao = @observacao
                                                 WHERE 
                                                 scunfa_idt = @IdAprovacao",
                                        parameters: new
                                        {
                                            IdAprovacao = idCienciaAprovador,
                                            flagRejeitar = saidaMaterialNotaFiscalCienciaModel.Ciente,
                                            observacao = saidaMaterialNotaFiscalCienciaModel.Observacao,

                                        },
                                        cancellationToken: cancellationToken,
                                        transaction: Transaction
                                    )
                                );

            return resultNF;
        }

        public async Task<TryException<int>> AtualizarCienciaStatus(SaidaMaterialNotaFiscalCienciaModel saidaMaterialNotaFiscalCienciaModel, CancellationToken cancellationToken)
        {
            var resultNF = await _db.Connection.QueryFirstOrDefaultAsync<int>(
                                new CommandDefinition(
                                    commandText: $@"UPDATE 
                                                 tb_socnf_solicitacao_ciencia_nota_fiscal
                                                 SET 
                                                 scinf_idt = @statusCiencia,
                                                 socnf_dat_atualizacao = GETDATE()
                                                 WHERE 
                                                 socnf_idt = @IdCiencia",
                                    parameters: new
                                    {
                                        IdCiencia = saidaMaterialNotaFiscalCienciaModel.Id,
                                        statusCiencia = saidaMaterialNotaFiscalCienciaModel.IdStatusCiencia
                                    },
                                    cancellationToken: cancellationToken,
                                    transaction: Transaction
                                )
                            );

            return resultNF;
        }

        private async Task<TryException<int>> InserirItens(int idSaidaMaterialNotaFiscalCiencia, List<SaidaMaterialNotaFiscalCienciaItemModel> ListaSaidaMaterialNotaFiscalCienciaItem, CancellationToken cancellationToken)
        {
            var listaIdSaidaMaterialNotaFiscalItem = new List<dynamic>();
            ListaSaidaMaterialNotaFiscalCienciaItem.ForEach(item => listaIdSaidaMaterialNotaFiscalItem.Add(new { idItem = item.IdSaidaMaterialNotaFiscalItem }));

            var query = $@"
                         INSERT INTO [dbo].[tb_scitnf_solicitacao_ciencia_item_nota_fiscal]
			                   (
                                 socnf_idt
                                ,smnfi_idt
                                ,scitnf_flg_ativo
                                ,scitnf_dat_criacao
                                ,scitnf_dat_atualizacao
                               )
		                 VALUES
			                   (
                                {idSaidaMaterialNotaFiscalCiencia}
			                   ,@idItem
			                   ,1
			                   ,GETDATE()
			                   ,GETDATE()
                            );";

            return await _db.Connection.ExecuteAsync(new CommandDefinition(
                    commandText: query,
                    parameters: listaIdSaidaMaterialNotaFiscalItem,
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                ));
        }

        private async Task<TryException<int>> InserirAprovadores(int idSaidaMaterialNotaFiscalCiencia, CancellationToken cancellationToken)
        {
            var listaGuidAprovadoresNF = await ListarGuidDeAprovadores(cancellationToken);

            if (listaGuidAprovadoresNF.IsFailure)
                return listaGuidAprovadoresNF.Failure;

            var query = $@"
                         insert into tb_scunfa_solicitacao_ciencia_usuario_nota_fiscal_aprovacao
                            (
                             socnf_idt
                            ,scunfa_usu_guid_ad
                            ,scunfa_flg_ativo
                            ,scunfa_dat_criacao
                            ,scunfa_dat_atualizacao
                            )
                         values 
                            (
                             {idSaidaMaterialNotaFiscalCiencia}
                            ,@guidUsuario
                            ,1
                            ,getdate()
                            ,getdate()
                            );";

            return await _db.Connection.ExecuteAsync(new CommandDefinition(
                    commandText: query,
                    parameters: listaGuidAprovadoresNF.Success,
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                ));
        }

        private async Task<TryException<List<dynamic>>> ListarGuidDeAprovadores(CancellationToken cancellationToken)
        {
            var listaGuidAprovadoresNF = new List<dynamic>();

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
                listaGuidAprovadoresNF.Add(new { guidUsuario = idUsuario });
            }

            return listaGuidAprovadoresNF;
        }

        public async Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarUsuarioAprovacaoPorId(int IdSolicitacaoCienciaNf, CancellationToken cancellationToken)
        {

            var resultNF = await _db.Connection.QueryAsync<CienciaUsuarioAprovacaoModel>(
                    new CommandDefinition(
                        commandText: $@"
                                 SELECT
                                     u.scunfa_idt as Id
                                    ,u.scunfa_usu_guid_ad as UsuarioGuid
                                    ,u.scunfa_des_observacao as Observacao
                                    ,u.scunfa_dat_aprovacao as DataAprovacao
                                    ,u.scunfa_flg_rejeitado as FlgRejeitado
                                 FROM 
                                    tb_scunfa_solicitacao_ciencia_usuario_nota_fiscal_aprovacao u
                                 WHERE
                                    u.scunfa_flg_rejeitado = 1
                                    AND u.socnf_idt = @IdSolicitacaoCienciaNf ",
                        parameters: new
                        {
                            IdSolicitacaoCienciaNf,
                        },
                        cancellationToken: cancellationToken
                    )
                );

            return resultNF.ToList();
        }
    }
}
