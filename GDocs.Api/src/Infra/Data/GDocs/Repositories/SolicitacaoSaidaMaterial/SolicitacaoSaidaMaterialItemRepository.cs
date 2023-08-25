using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories.SolicitacaoSaidaMaterial
{
    internal class SolicitacaoSaidaMaterialItemRepository : Repository, ISolicitacaoSaidaMaterialItemRepository
    {
        public SolicitacaoSaidaMaterialItemRepository(IGDocsDatabase db, IUnitOfWork unitOfWork) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<SolicitacaoSaidaMaterialItemModel>> ObterItemPorId(int idSolicitacaoSaidaMaterialItem, CancellationToken cancellationToken)
        {
            var query = @"SELECT 
                                [ssmi_idt] as IdSolicitacaoSaidaMaterialItem
                               ,[ssm_idt] as  IdSolicitacaoSaidaMaterial
                               ,[ssmi_qtd_item] as Quantidade
                               ,[ssmi_unidade] as Unidade
                               ,[ssm_num_patrimonio] as Patrimonio
                               ,[ssmi_descricao] as Descricao
                               ,[ssmi_dat_criacao] as DataCriacao
                               ,[ssmi_flg_ativo] as Ativo
                               ,[ssmi_dat_atualizacao] AS DataAtualizacao
                          FROM [dbo].[tb_ssmi_solicitacao_saida_material_item]
                          WHERE [ssmi_idt] = @IdSolicitacaoSaidaMaterialItem";

            var result = await _db.Connection.QueryFirstOrDefaultAsync<SolicitacaoSaidaMaterialItemModel>(new CommandDefinition(
                    commandText: query,
                    parameters: new { IdSolicitacaoSaidaMaterialItem = idSolicitacaoSaidaMaterialItem },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );

            return result;
        }

        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialItemModel>>> ObterPorIdSolicitacaoSaidaMaterial(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<SolicitacaoSaidaMaterialItemModel>(
            new CommandDefinition(
                commandText: $@"
                           SELECT [ssmi_idt] as IdSolicitacaoSaidaMaterialItem
                           ,[ssm_idt] as  IdSolicitacaoSaidaMaterial
                           ,[ssmi_qtd_item] as Quantidade
                           ,[ssmi_unidade] as Unidade
                           ,[ssm_num_patrimonio] as Patrimonio
                           ,[ssmi_descricao] as Descricao
                           ,[ssmi_dat_criacao] as DataCriacao
                           ,[ssmi_flg_ativo] as Ativo
                           FROM [dbo].[tb_ssmi_solicitacao_saida_material_item] WHERE ssm_idt = @idSolicitacaoSaidaMaterial",
                    parameters: new
                    {
                        idSolicitacaoSaidaMaterial,
                    },
                    transaction: Transaction
               )
           );
            return result?.ToCollection();
        }

       

        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialItemModel>>> ObterPorIdSolicitacaoSaidaMaterialRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {

            var result = await _db.Connection.QueryAsync<SolicitacaoSaidaMaterialItemModel>(
            new CommandDefinition(
                commandText: $@"
                               SELECT	
			                        DISTINCT(ssmi.[ssmi_idt]) as IdSolicitacaoSaidaMaterialItem 
			                        ,ssmi.[ssm_idt] as  IdSolicitacaoSaidaMaterial
			                        ,[ssmi_qtd_item] as Quantidade
			                        ,[ssmi_unidade] as Unidade
			                        ,[ssm_num_patrimonio] as Patrimonio
			                        ,[ssmi_descricao] as Descricao
			                        ,[ssmi_dat_criacao] as DataCriacao
			                        ,[ssmi_flg_ativo] as Ativo
		                        FROM 
			                        [dbo].[tb_ssmi_solicitacao_saida_material_item] ssmi
		                        LEFT JOIN 
			                        [dbo].[tb_ssmai_solicitacao_saida_material_acao_item] ssmai
			                        on ssmai.ssmi_idt = ssmi.ssmi_idt
		                        LEFT JOIN 
			                        [dbo].[tb_ssma_solicitacao_saida_material_acao] ssma 
			                        on ssma.ssma_idt = ssmai.ssma_idt
		                        LEFT JOIN 
			                        [dbo].[tb_smta_saida_material_tipo_acao] smta 
			                        on smta.smta_idt = ssma.smta_idt
		                        WHERE 
			                        (ssma.ssm_idt = @idSolicitacaoSaidaMaterial AND smta.smta_idt = {(int)SaidaMaterialTipoAcao.RegistroSaida}) 
                                OR 
                                    (ssma.ssm_idt = @idSolicitacaoSaidaMaterial AND smta.smta_idt = {(int)SaidaMaterialTipoAcao.SolicitacaoProrrogacao})
		                        GROUP BY
			                        ssmi.[ssmi_idt]
			                        ,ssmi.[ssm_idt]
			                        ,[ssmi_qtd_item]
			                        ,[ssmi_unidade]
			                        ,[ssm_num_patrimonio]
			                        ,[ssmi_descricao]
			                        ,[ssmi_dat_criacao]
			                        ,[ssmi_flg_ativo]",
                    parameters: new
                    {
                        idSolicitacaoSaidaMaterial,
                    }
               )
           );
            return result?.ToCollection();
        }

        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialItemProrrogadoModel>>> ObterItensProrrogadosEAprovados(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {

            var result = await _db.Connection.QueryAsync<SolicitacaoSaidaMaterialItemProrrogadoModel>(
            new CommandDefinition(
                commandText: $@"
                               SELECT 
	                                COUNT(ssmi_idt) AS TotalAprovadores, ssmi_idt AS IdSolicitacaoSaidaMaterialItem
                                FROM 
	                                tb_scua_solicitacao_ciencia_usuario_aprovacao scua
                                INNER JOIN 
	                                tb_soc_solicitacao_ciencia soc ON soc.soc_idt = scua.soc_idt
                                INNER JOIN
	                                tb_scit_solicitacao_ciencia_item scit ON  scit.soc_idt = soc.soc_idt
                                WHERE 
	                                soc.ssm_idt = @idSolicitacaoSaidaMaterial
                                AND 
	                                scua_dat_aprovacao IS NOT NULL
                                GROUP BY 
	                                ssmi_idt",
                    parameters: new
                    {
                        idSolicitacaoSaidaMaterial,
                    }
               )
           );
            return result?.ToCollection();
        }

        public async Task<TryException<Return>> DesativarItemMaterial(IEnumerable<int> ssmiIdt, CancellationToken cancellationToken)
        {
                var command = @"DELETE 
                                    tb_ssmi_solicitacao_saida_material_item
                               WHERE
                                    ssmi_idt in @ssmiIdt";

            await _db.Connection.ExecuteAsync(new CommandDefinition(
                    commandText: command,
                    parameters: new
                         {
                            ssmiIdt
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );

            return Return.Empty;
        }



        public async Task<TryException<Return>> Inserir(SolicitacaoSaidaMaterialItemModel solicitacaoSaidaMaterialItemModel, CancellationToken cancellationToken)
        {
            var command = @" MERGE INTO [dbo].tb_ssmi_solicitacao_saida_material_item AS Target
				USING
				(
					   VALUES
                           (@IdSolicitacaoSaidaMaterialItem
						   ,@IdSolicitacaoSaidaMaterial
                           ,@Quantidade
                           ,@Unidade
                           ,@Patrimonio
                           ,@Descricao
                           ,GETDATE()
                           ,1)
				) AS Source 
			       ([ssmi_idt], [ssm_idt] ,[ssmi_qtd_item] ,[ssmi_unidade] ,[ssm_num_patrimonio] ,[ssmi_descricao] ,[ssmi_dat_criacao]
                    ,[ssmi_flg_ativo])
					
				ON Target.[ssmi_idt] = Source.[ssmi_idt]

				--altera registros que existem no Target e no Source
				WHEN MATCHED THEN
					UPDATE SET 
                             [ssmi_qtd_item] = Source.[ssmi_qtd_item]
                            ,[ssmi_unidade] = Source.[ssmi_unidade]
                            ,[ssm_num_patrimonio] = Source.[ssm_num_patrimonio]
                            ,[ssmi_descricao] = Source.[ssmi_descricao]
                            ,[ssmi_dat_criacao] = GETDATE()                                       
                            ,[ssmi_flg_ativo] = Source.[ssmi_flg_ativo]                                                                                

				--inseri novos registros que não existem no target e existem no source
				WHEN NOT MATCHED BY TARGET THEN 
					INSERT 
                           ([ssm_idt]
                           ,[ssmi_qtd_item]
                           ,[ssmi_unidade]
                           ,[ssm_num_patrimonio]
                           ,[ssmi_descricao]
                           ,[ssmi_dat_criacao]
                           ,[ssmi_flg_ativo])
                        VALUES
                           ([ssm_idt]
                           ,[ssmi_qtd_item]
                           ,[ssmi_unidade]
                           ,[ssm_num_patrimonio]
                           ,[ssmi_descricao]
                           ,[ssmi_dat_criacao]
                           ,[ssmi_flg_ativo])
                OUTPUT
                    INSERTED.[ssmi_idt];";

            await _db.Connection.ExecuteAsync(new CommandDefinition(
                    commandText: command,
                    parameters: solicitacaoSaidaMaterialItemModel,
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );

            return Return.Empty;
        }

        public async Task<TryException<SolicitacaoSaidaMaterialItemModel>> Atualizar(SolicitacaoSaidaMaterialItemModel solicitacaoSaidaMaterialItemModel)
        {
            var command = @"UPDATE [dbo].[tb_ssmi_solicitacao_saida_material_item]
                           SET [ssm_idt] = @IdSolicitacaoSaidaMaterial
                              ,[ssmi_qtd_item] = @Quantidade
                              ,[ssmi_unidade] = @Unidade
                              ,[ssm_num_patrimonio] = @Patrimonio
                              ,[ssmi_descricao] = @Descricao
                              ,[ssmi_flg_ativo] = @Ativo
                              ,[ssmi_dat_atualização] = @DataAtualizacao
                         WHERE ssmi_idt = @Id";

            await _db.Connection.ExecuteAsync(command, solicitacaoSaidaMaterialItemModel);

            return solicitacaoSaidaMaterialItemModel;
        }

        public async Task<TryException<Return>> Excluir(int id)
        {
            var command = @"DELETE FROM [dbo].[tb_ssmi_solicitacao_saida_material_item] WHERE ssmi_idt = @Id";
            await _db.Connection.ExecuteAsync(command, new { id });
            return Return.Empty;
        }

        public async Task<TryException<int>> ObterQuantidadeItemComBaixa(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {
            return await _db.Connection.QueryFirstOrDefaultAsync<int>(new CommandDefinition(
                    commandText: $@"SELECT 
									COUNT(SSM.ssm_idt) as [Quantidade] 
									    FROM 
									tb_ssm_solicitacao_saida_material ssm 
									INNER JOIN 
									tb_ssma_solicitacao_saida_material_acao ssma 
									ON ssma.ssm_idt = ssm.ssm_idt
									INNER JOIN 
									tb_ssmai_solicitacao_saida_material_acao_item ssmai
									ON  ssmai.ssma_idt = ssma.ssma_idt
									    WHERE
									ssma.ssm_idt = @idSolicitacaoSaidaMaterial AND 
									ssma.smta_idt IN ({(int)SaidaMaterialTipoAcao.RegistroRetorno},{(int)SaidaMaterialTipoAcao.BaixaMaterialSemRetorno})",
                    parameters: new
                    {
                        idSolicitacaoSaidaMaterial
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                ));
        }

        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialItemModel>>> ObterItemMateriaParaRetornoPorId(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<SolicitacaoSaidaMaterialItemModel>(
            new CommandDefinition(
                commandText: $@"SELECT 
									    ssmi.[ssmi_idt] as IdSolicitacaoSaidaMaterialItem
									   ,ssmi.[ssm_idt] as  IdSolicitacaoSaidaMaterial
									   ,ssmi.[ssmi_qtd_item] as Quantidade
									   ,ssmi.[ssmi_unidade] as Unidade
									   ,ssmi.[ssm_num_patrimonio] as Patrimonio
									   ,ssmi.[ssmi_descricao] as Descricao
									   ,ssmi.[ssmi_dat_criacao] as DataCriacao
									   ,ssmi.[ssmi_flg_ativo] as Ativo
								FROM
                                       tb_ssmi_solicitacao_saida_material_item ssmi 
								WHERE 
								    ssmi.ssm_idt = @idSolicitacaoSaidaMaterial AND
									ssmi.ssmi_idt NOT IN ((
									SELECT 
									ssmai.ssmi_idt 
									FROM 
									tb_ssm_solicitacao_saida_material ssm 
									INNER JOIN 
									tb_ssma_solicitacao_saida_material_acao ssma 
									ON ssma.ssm_idt = ssm.ssm_idt
									INNER JOIN 
									tb_ssmai_solicitacao_saida_material_acao_item ssmai
									ON  ssmai.ssma_idt = ssma.ssma_idt
									WHERE
									ssma.ssm_idt = @idSolicitacaoSaidaMaterial AND 
									ssma.smta_idt  IN ({(int)SaidaMaterialTipoAcao.RegistroRetorno},{(int)SaidaMaterialTipoAcao.BaixaMaterialSemRetorno})))",
                    parameters: new
                    {
                        idSolicitacaoSaidaMaterial,
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
               ));
            return result?.ToCollection();
        }

    }
}
