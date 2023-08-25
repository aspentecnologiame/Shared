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
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories.SolicitacaoSaidaMaterial
{
    internal class SaidaMaterialNotaFiscalAcaoRepository : Repository, ISaidaMaterialNotaFiscalAcaoRepository
    {
        public SaidaMaterialNotaFiscalAcaoRepository(IGDocsDatabase db, IUnitOfWork unitOfWork) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalTipoAcaoModel>>> ListarAcaoTipo()
        {
            var query = @"SELECT [smtanf_idt] AS Id
                          ,[smtanf_des] as Descricao
                          ,[smtanf_flg_ativo] as FlgAtivo
                          ,[smtanf_dat_criacao] as DataCriacao
                          ,[smtanf_dat_atualizacao] as DataAtualizacao
                      FROM [DB_SP_GDOCS].[dbo].[tb_smtanf_saida_material_tipo_acao_nota_fiscal]";

            var result = await _db.Connection.QueryAsync<SaidaMaterialNotaFiscalTipoAcaoModel>(query);

            return result?.ToCollection();
        }

        public async Task<TryException<int>> InserirAcao(SaidaMaterialNotaFiscalAcaoModel saidaMaterialNotaFiscalAcaoModel, CancellationToken cancellationToken)
        {
            var command = @"INSERT INTO [dbo].[tb_smnfa_saida_material_nota_fiscal_acao]
			                   ([smnf_idt]
			                   ,[smtanf_idt]
			                   ,[smnfa_nom_conferente]
			                   ,[smnfa_dat_acao]
			                   ,[smnfa_nom_portador]
			                   ,[smnfa_des_setor_empresa]
			                   ,[smnfa_des_observacao]
			                   ,[smnfa_flg_ativo]
			                   ,[smnfa_dat_criacao]
			                   ,[smnfa_dat_atualizacao])
		                 VALUES
			                   (@IdSaidaMaterialNotaFiscal
			                   ,@IdSaidaMaterialNotaFiscalTipoAcao
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
                    parameters: saidaMaterialNotaFiscalAcaoModel,
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );

            return result;
        }

        public async Task<TryException<bool>> InserirAcaoItem(SaidaMaterialNotaFiscalAcaoItemModel saidaMaterialNotaFiscalAcaoItemModel, CancellationToken cancellationToken)
        {
            var command = @"INSERT INTO [dbo].[tb_smnfai_saida_material_nota_fiscal_acao_item]
                               ([smnf_idt]
                               ,[smnfa_idt]
                               ,[smnfi_idt]
                               ,[smnfai_flg_ativo]
                               ,[smnfai_dat_criacao]
                               ,[smnfai_dat_atualizacao])
                         VALUES
                               (@IdSolicitacaoSaidaMaterialNF
                               ,@IdSaidaMaterialNotaFiscalAcao
                               ,@IdSaidaMaterialNotaFiscalItem
                               ,@FlgAtivo
                               ,GETDATE()
                               ,GETDATE())";

            var result = await _db.Connection.ExecuteAsync(new CommandDefinition(
                    commandText: command,
                    parameters: saidaMaterialNotaFiscalAcaoItemModel,
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );

            return result > 0;
        }

        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalAcaoItemModel>>> ObterItensPorIdSolicitacaoSaidaMaterialETipoAcao(int idSaidaMaterialNotaFiscal, SaidaMaterialNotaFiscalTipoAcao saidaMaterialTiposAcao, CancellationToken cancellationToken)
        {
            var query = @"SELECT [smnfai_idt] AS Id
								  ,[smnfai].[smnfa_idt] as IdSaidaMaterialNotaFiscalAcao
								  ,[smnfi_idt] AS IdSaidaMaterialNotaFiscalItem
								  ,[smnfai_flg_ativo] AS FlgAtivo
								  ,[smnfai_dat_criacao] AS DataCriacao
								  ,[smnfai_dat_atualizacao] AS DataAtualizacao
							FROM 
	                            [dbo].[tb_smnfa_saida_material_nota_fiscal_acao] smnfa
	                            INNER JOIN [dbo].[tb_smnfai_saida_material_nota_fiscal_acao_item] smnfai
	                            ON smnfai.smnfa_idt = smnfa.smnfa_idt
                            WHERE
	                            smnfa.smnf_idt = @idSaidaMaterialNotaFiscal
	                            AND smnfa.smtanf_idt = @saidaMaterialTiposAcao";

            var result = await _db.Connection.QueryAsync<SaidaMaterialNotaFiscalAcaoItemModel>(new CommandDefinition(
                    commandText: query,
                    parameters: new { idSaidaMaterialNotaFiscal, saidaMaterialTiposAcao },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );

            return result?.ToCollection();
        }

        public async Task<TryException<IEnumerable<HistoricoProrrogacaoNotaFiscalModel>>> ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(int idSaidaMaterialNotaFiscal, SaidaMaterialNotaFiscalTipoAcao tipoAcao, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<HistoricoProrrogacaoNotaFiscalModel>(new CommandDefinition(
                commandText: @"
                   SELECT 
	                   hspnf.hspnf_dat_de as DataRetornoAtual,
	                   hspnf.hspnf_dat_para as DataRetornoNova,
	                   smnfa.smnfa_des_observacao as  Observacao,
                       dbo.fn_retorna_obs_diretoria_acao_NF(socnf.socnf_idt) as HtmlInfoDir,
	                   scinf.scinf_des as  Status,
                       scinf.scinf_idt as StatusId
	               FROM 
                       tb_hspnf_historico_solicitacao_prorogacao_nota_fiscal hspnf
                       INNER JOIN tb_smnfa_saida_material_nota_fiscal_acao smnfa on smnfa.smnfa_idt = hspnf.smnfa_idt  
	                   INNER JOIN tb_socnf_solicitacao_ciencia_nota_fiscal socnf on socnf.smnfa_idt = hspnf.smnfa_idt
	                   INNER JOIN tb_scinf_status_ciencia_nota_fiscal scinf on scinf.scinf_idt = socnf.scinf_idt
	               WHERE  
	                   hspnf.smnf_idt = @idSaidaMaterialNotaFiscal
	                   AND smnfa.smtanf_idt = @tipoAcao
	               ORDER BY 
                        hspnf.hspnf_dat_criacao
                   ASC",
                parameters: new
                {
                    idSaidaMaterialNotaFiscal,
                    tipoAcao
                },
                transaction: Transaction,
                cancellationToken: cancellationToken
                )
            );

            return result.ToCollection();
        }

        public async Task<TryException<DateTime?>> ObterDataDeRetornoDaSolicitacaoDeSaidaDeMaterial(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken)
        {
            return await _db.Connection.QueryFirstOrDefaultAsync<DateTime?>(new CommandDefinition(
                    commandText: @"SELECT
                                     smnf_dat_retorno as [DataRetorno]
                                   FROM
                                     [dbo].[tb_smnf_saida_material_nota_fiscal]
                                   WHERE
                                    smnf_idt = @idSaidaMaterialNotaFiscal",
                    parameters: new
                    {
                        idSaidaMaterialNotaFiscal
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );
        }
        public async Task<TryException<DateTime?>> ObterDataOriginalDeRetornoDaSolicitacaoDeSaidaDeMaterial(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken)
        {
            return await _db.Connection.QueryFirstOrDefaultAsync<DateTime?>(new CommandDefinition(
                    commandText: @"SELECT top 1
                                     smnf_dat_retorno as [DataRetorno]
                                   FROM
                                     [dbo].[tb_smnf_saida_material_nota_fiscal]
                                   WHERE
                                   smnf_idt = @idSaidaMaterialNotaFiscal
                                   ORDER BY smnf_dat_retorno",
                    parameters: new
                    {
                        idSaidaMaterialNotaFiscal
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );
        }

        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalAcaoModel>>> LitarAcaoSaidaEhRetorno(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<SaidaMaterialNotaFiscalAcaoModel>(new CommandDefinition(
            commandText: @"
                        SELECT  
                            smnfa.smnfa_idt [Id],
                            smnfa.smnf_idt [IdSaidaMaterialNotaFiscal],
                            smnfa.smtanf_idt [IdSaidaMaterialNotaFiscalTipoAcao],  
                            smnfai.smnfi_idt [IdSaidaMaterialNotaFiscalAcaoItem],
                            smnfa.smnfa_nom_conferente [Conferente],
                            smnfa.smnfa_dat_acao [DataAcao],
                            smnfa.smnfa_nom_portador [Portador],
                            smnfa.smnfa_des_setor_empresa [SetorEmpresa],
                            smnfa.smnfa_des_observacao [Observacao],
                            smtanf.smtanf_des [DescricaoStatus],
                            smnfa.smnfa_flg_ativo [FlgAtivo]
                        FROM
                            tb_smnfa_saida_material_nota_fiscal_acao smnfa 
                            INNER JOIN tb_smtanf_saida_material_tipo_acao_nota_fiscal smtanf 
                            ON smtanf.smtanf_idt = smnfa.smtanf_idt
                            INNER JOIN tb_smnfai_saida_material_nota_fiscal_acao_item smnfai
                            ON smnfai.smnfa_idt = smnfa.smnfa_idt
                        WHERE
                            smnfa.smnf_idt = @idSaidaMaterialNotaFiscal
                            AND smtanf.smtanf_idt in (1,2)
                            AND smnfa.smnfa_flg_ativo = 1
                            AND smnfai_flg_ativo = 1",
            parameters: new
            {
                idSaidaMaterialNotaFiscal,
            },
            transaction: Transaction,
            cancellationToken: cancellationToken
            ));


            return result.ToCollection();
        }

        public async Task<TryException<IEnumerable<HistoricoProrrogacaoNotaFiscalModel>>> ListarHistoricoBaixaSemRetorno(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<HistoricoProrrogacaoNotaFiscalModel>(new CommandDefinition(
                commandText: @"
                   SELECT 
                       smnfai.smnfi_idt as SaidaMaterialNotaFiscalItemId,
	                   hspnf.hspnf_dat_de as DataRetornoAtual,
	                   hspnf.hspnf_dat_para as DataRetornoNova,
	                   smnfa.smnfa_des_observacao as  Observacao,
                       dbo.fn_retorna_obs_diretoria_acao_NF(socnf.socnf_idt) as HtmlInfoDir,
	                   scinf.scinf_des as  Status,
	                   scinf.scinf_idt as StatusId
	               FROM 
                       tb_hspnf_historico_solicitacao_prorogacao_nota_fiscal hspnf
                       INNER JOIN tb_smnfa_saida_material_nota_fiscal_acao smnfa on smnfa.smnfa_idt = hspnf.smnfa_idt  
                       INNER JOIN tb_smnfai_saida_material_nota_fiscal_acao_item smnfai on smnfa.smnfa_idt = smnfai.smnfa_idt
	                   INNER JOIN tb_socnf_solicitacao_ciencia_nota_fiscal socnf on socnf.smnfa_idt = hspnf.smnfa_idt
	                   INNER JOIN tb_scinf_status_ciencia_nota_fiscal scinf on scinf.scinf_idt = socnf.scinf_idt
	               WHERE  
	                   hspnf.smnf_idt = @idSaidaMaterialNotaFiscal
	                   AND smnfa.smtanf_idt = @tipoAcao
	               ORDER BY 
                        hspnf.hspnf_dat_criacao
                   ASC",
                parameters: new
                {
                    idSaidaMaterialNotaFiscal,
                    tipoAcao = SaidaMaterialNotaFiscalTipoAcao.SolicitacaoBaixaSemRetorno
                },
                transaction: Transaction,
                cancellationToken: cancellationToken
                )
            );

            return result.ToCollection();
        }

        public async Task<TryException<IEnumerable<HistoricoTrocaAnexoSaidaModel>>> ListarHistoricoAnexoSaida(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<HistoricoTrocaAnexoSaidaModel>(new CommandDefinition(
                  commandText: @"
                                SELECT 
                                     smdnf_idt AS Id,
                                     smnf_numero_documento AS Numero,
                                     smnf_motivo AS Motivo,
                                     smdnf_dat_alteracao AS Alteracao,
                                     smdnf_guid_ad_autor AS GuidAutor
                                FROM
                                    tb_smdnf_saida_material_documento_nota_fiscal
                                WHERE
                                    smnf_idt = @idSolicitacaoSaidaMaterial
                                    AND smdnf_flg_ativo = 0
                                    AND tdnf_idt = @tipoAcao
                               ORDER BY 
                                    smdnf_dat_alteracao 
                               DESC",
                  parameters: new
                  {
                      idSolicitacaoSaidaMaterial,
                      tipoAcao = TipoDocAnexo.Saida
                  },
                  transaction: Transaction,
                  cancellationToken: cancellationToken
                  )
              );

            return result.ToCollection();
        }
    }
}
