using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Infra.Data.Core.Repositories;
using Dapper;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Threading;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories.SolicitacaoSaidaMaterial
{
    internal class SolicitacaoSaidaMaterialAcaoRepository : Repository, ISolicitacaoSaidaMaterialAcaoRepository
    {
        public SolicitacaoSaidaMaterialAcaoRepository(IGDocsDatabase db, IUnitOfWork unitOfWork) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialAcaoTipoModel>>> ListarAcaoTipo()
        {
            var query = @"SELECT [smta_idt] AS Id
                          ,[smta_des] as Descricao
                          ,[smta_flg_ativo] as FlgAtivo
                          ,[smta_dat_criacao] as DataCriacao
                          ,[smta_dat_atualizacao] as DataAtualizacao
                      FROM [DB_SP_GDOCS].[dbo].[tb_smta_saida_material_tipo_acao]";

            var result = await _db.Connection.QueryAsync<SolicitacaoSaidaMaterialAcaoTipoModel>(query);

            return result?.ToCollection();
        }

        public async Task<TryException<int>> InserirAcao(SolicitacaoSaidaMaterialAcaoModel solicitacaoSaidaMaterialAcaoModel, CancellationToken cancellationToken)
        {
                var command = @"INSERT INTO [dbo].[tb_ssma_solicitacao_saida_material_acao]
			                   ([ssm_idt]
			                   ,[smta_idt]
			                   ,[ssma_nom_conferente]
			                   ,[ssma_dat_acao]
			                   ,[ssma_nom_portador]
			                   ,[ssma_des_setor_empresa]
			                   ,[ssma_des_observacao]
			                   ,[ssma_flg_ativo]
			                   ,[ssma_dat_criacao]
			                   ,[ssma_dat_atualizacao])
		                 VALUES
			                   (@IdSolicitacaoSaidaMaterial
			                   ,@IdSaidaMaterialTipoAcao
			                   ,@Conferente
			                   ,@DataAcao
			                   ,@Portador
			                   ,@SetorEmpresa
			                   ,@Observacao
			                   ,@FlgAtivo
			                   ,GETDATE()
			                   ,GETDATE());
                            SELECT CAST(SCOPE_IDENTITY() as [int]);";

                var result = await _db.Connection.QueryFirstAsync<int>(new CommandDefinition(
                        commandText: command,
                        parameters: solicitacaoSaidaMaterialAcaoModel,
                        transaction: Transaction,
                        cancellationToken: cancellationToken
                    )
                );

                return result;
        }

		public async Task<TryException<bool>> InserirAcaoItem(SolicitacaoSaidaMaterialAcaoItemModel solicitacaoSaidaMaterialAcaoItemModel, CancellationToken cancellationToken)
		{
			var command = @"INSERT INTO [dbo].[tb_ssmai_solicitacao_saida_material_acao_item]
                               ([ssma_idt]
                               ,[ssmi_idt]
                               ,[ssmai_flg_ativo]
                               ,[ssmai_dat_criacao]
                               ,[ssmai_dat_atualizacao])
                         VALUES
                               (@IdSolicitacaoSaidaMaterialAcao
                               ,@IdSolicitacaoSaidaMaterialItem
                               ,@FlgAtivo
                               ,GETDATE()
                               ,GETDATE())";

			var result = await _db.Connection.ExecuteAsync(new CommandDefinition(
					commandText: command,
					parameters: solicitacaoSaidaMaterialAcaoItemModel,
					transaction: Transaction,
					cancellationToken: cancellationToken
				)
			);

			return result > 0;
		}

        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialAcaoItemModel>>> ObterItensPorIdSolicitacaoSaidaMaterialETipoAcao(int idSolicitacaoSaidaMaterial, SaidaMaterialTipoAcao saidaMaterialTiposAcao, CancellationToken cancellationToken)
        {
            var query = @"SELECT [ssmai_idt] AS Id
								  ,[ssmai].[ssma_idt] as IdSolicitacaoSaidaMaterialAcao
								  ,[ssmi_idt] AS IdSolicitacaoSaidaMaterialItem
								  ,[ssmai_flg_ativo] AS FlgAtivo
								  ,[ssmai_dat_criacao] AS DataCriacao
								  ,[ssmai_dat_atualizacao] AS DataAtualizacao
							FROM 
	                            [dbo].[tb_ssma_solicitacao_saida_material_acao] ssma
	                            INNER JOIN [dbo].[tb_ssmai_solicitacao_saida_material_acao_item] ssmai
	                            ON ssmai.ssma_idt = ssma.ssma_idt
                            WHERE
	                            ssma.ssm_idt = @idSolicitacaoSaidaMaterial
	                            AND ssma.smta_idt = @saidaMaterialTiposAcao";

            var result = await _db.Connection.QueryAsync<SolicitacaoSaidaMaterialAcaoItemModel>(new CommandDefinition(
                    commandText: query,
                    parameters: new { idSolicitacaoSaidaMaterial, saidaMaterialTiposAcao },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );

            return result?.ToCollection();
        }

        public async Task<TryException<IEnumerable<HistoricoProggoracaoModel>>> ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(int idSolicitacaoSaidaMaterial, SaidaMaterialTipoAcao tipoAcao, CancellationToken cancellationToken)
        {
			var result = await _db.Connection.QueryAsync<HistoricoProggoracaoModel>(new CommandDefinition(
				commandText: @"
                   SELECT 
	                   hsp.hsp_dat_de as DataRetornoAtual,
	                   hsp.hsp_dat_para as DataRetornoNova,
	                   ssma.ssma_des_observacao as  Observacao,
                        dbo.fn_retorna_obs_diretoria_acao_sem_NF(soc.soc_idt) as HtmlInfoDir,
	                   sci.sci_des as  Status
	               FROM 
                       tb_hsp_historico_solicitacao_prorogacao hsp
                       INNER JOIN tb_ssma_solicitacao_saida_material_acao ssma on ssma.ssma_idt = hsp.ssma_idt  
	                   INNER JOIN tb_soc_solicitacao_ciencia soc on soc.ssma_idt = hsp.ssma_idt
	                   INNER JOIN  tb_sci_status_ciencia sci on sci.sci_idt = soc.sci_idt
	               WHERE  
	                   hsp.ssm_idt = @idSolicitacaoSaidaMaterial
	                   AND ssma.smta_idt = @tipoAcao
	               ORDER BY 
                        hsp.hsp_dat_criacao
                   ASC",
				parameters: new
				{
					idSolicitacaoSaidaMaterial,
                    tipoAcao
                },
				transaction: Transaction,
				cancellationToken: cancellationToken
			    )
		    );

			return result.ToCollection();
		}

        public async Task<TryException<DateTime?>> ObterDataDeRetornoDaSolicitacaoDeSaidaDeMaterial(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {
            return await _db.Connection.QueryFirstOrDefaultAsync<DateTime?>(new CommandDefinition(
                    commandText: @"SELECT
                                     ssm_dat_retorno as [DataRetorno]
                                   FROM
                                     [dbo].[tb_ssm_solicitacao_saida_material]
                                   WHERE
                                    ssm_idt = @idSolicitacaoSaidaMaterial",
                    parameters: new { 
                        idSolicitacaoSaidaMaterial 
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );
        }
        public async Task<TryException<DateTime?>> ObterDataOriginalDeRetornoDaSolicitacaoDeSaidaDeMaterial(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {
            return await _db.Connection.QueryFirstOrDefaultAsync<DateTime?>(new CommandDefinition(
                    commandText: @"SELECT top 1
                                     ssm_dat_retorno as [DataRetorno]
                                   FROM
                                     [dbo].tb_ssm_solicitacao_saida_material
                                   WHERE
                                   ssm_idt = @idSolicitacaoSaidaMaterial
                                   ORDER BY ssm_dat_retorno",
                    parameters: new { 
                        idSolicitacaoSaidaMaterial 
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );
        }

        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialAcaoModel>>> LitarAcaoSaidaEhRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<SolicitacaoSaidaMaterialAcaoModel>(new CommandDefinition(
            commandText: @"
                        SELECT  
                            SSMA.ssma_idt [Id],
                            SSMA.ssm_idt [IdSolicitacaoSaidaMaterial],
                            SMTA.smta_idt [IdSaidaMaterialTipoAcao],  
                            SSMAI.ssmi_idt [SolicitacaoMaterialAcaoItemId],
                            SSMA.SSMA_nom_conferente [Conferente],
                            SSMA.SSMA_dat_acao [DataAcao],
                            SSMA.SSMA_nom_portador [Portador],
                            SSMA.SSMA_des_setor_empresa [SetorEmpresa],
                            SSMA.SSMA_des_observacao [Observacao],
                            SMTA.SMTA_des [DescricaoStatus],
                            SSMA.ssma_flg_ativo [FlgAtivo]
                        FROM
                            tb_SSMA_solicitacao_saida_material_acao SSMA 

                            INNER JOIN tb_SMTA_saida_material_tipo_acao SMTA 
                            ON SMTA.SMTA_idt = SSMA.SMTA_idt

                            INNER JOIN tb_SSMAI_solicitacao_saida_material_acao_item SSMAI 
                            ON SSMAI.SSMA_idt = SSMA.SSMA_idt

                        WHERE
                            SSMA.ssm_idt = @idSolicitacaoSaidaMaterial
                            AND SMTA.SMTA_idt in (1,2)
                            AND SSMA.SSMA_flg_ativo = 1
                            AND SSMAI_flg_ativo = 1",
            parameters: new
            {
                idSolicitacaoSaidaMaterial,
            },
            transaction: Transaction,
            cancellationToken: cancellationToken
            ));


            return result.ToCollection();
        }

        public async Task<TryException<IEnumerable<HistoricoProggoracaoModel>>> ListarHistoricoBaixaSemRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<HistoricoProggoracaoModel>(new CommandDefinition(
                commandText: @"
                   SELECT 
                       ssmai.ssmi_idt as SolicitacaoMaterialItemId,
	                   hsp.hsp_dat_de as DataRetornoAtual,
	                   hsp.hsp_dat_para as DataRetornoNova,
	                   ssma.ssma_des_observacao as  Observacao,
                       dbo.fn_retorna_obs_diretoria_acao_sem_NF(soc.soc_idt) as HtmlInfoDir,
	                   sci.sci_des as  Status,
	                   sci.sci_idt as  StatusId
	               FROM 
                       tb_hsp_historico_solicitacao_prorogacao hsp
                       INNER JOIN tb_ssma_solicitacao_saida_material_acao ssma on ssma.ssma_idt = hsp.ssma_idt  
                       INNER JOIN tb_ssmai_solicitacao_saida_material_acao_item ssmai on ssma.ssma_idt = ssmai.ssma_idt
	                   INNER JOIN tb_soc_solicitacao_ciencia soc on soc.ssma_idt = hsp.ssma_idt
	                   INNER JOIN  tb_sci_status_ciencia sci on sci.sci_idt = soc.sci_idt
	               WHERE  
	                   hsp.ssm_idt = @idSolicitacaoSaidaMaterial
	                   AND ssma.smta_idt = @tipoAcao
	               ORDER BY 
                        hsp.hsp_dat_criacao
                   ASC",
                parameters: new
                {
                    idSolicitacaoSaidaMaterial,
                    tipoAcao = SaidaMaterialTipoAcao.SolicitacaoBaixaSemRetorno
                },
                transaction: Transaction,
                cancellationToken: cancellationToken
                )
            );

            return result.ToCollection();
        }
    }
}
