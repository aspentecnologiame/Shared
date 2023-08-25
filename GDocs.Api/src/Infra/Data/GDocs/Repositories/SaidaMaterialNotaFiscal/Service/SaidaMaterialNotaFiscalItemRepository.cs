using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.Data.Core.Repositories;
using ICE.GDocs.Infra.Data.Core.UoW;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories.SaidaMaterialNotaFiscal.Service
{
    internal class SaidaMaterialNotaFiscalItemRepository : Repository, ISaidaMaterialNotaFiscalItemRepository
    {
        public SaidaMaterialNotaFiscalItemRepository(IGDocsDatabase db, IUnitOfWork unitOfWork) : base(db, unitOfWork)
        {

        }

        public async Task<TryException<SaidaMaterialNotaFiscalItemModel>> ObterItemPorId(int idSaidaMaterialNotaFiscalItem, CancellationToken cancellationToken)
        {
            var query = @"SELECT 
                                [smnfi_idt] as IdSaidaMaterialNotaFiscalItem
                               ,[smnf_idt] as  IdSaidaMaterialNotaFiscal
                               ,[smnfi_qtd_item] as Quantidade
                               ,[smnfi_unidade] as Unidade
                               ,[smnfi_patrimonio] as Patrimonio
                               ,[smnfi_descricao] as Descricao
                               ,[smnfi_dat_criacao] as DataCriacao
                               ,[smnfi_flg_ativo] as Ativo
                               ,[smnfi_dat_atualizacao] AS DataAtualizacao
                          FROM [dbo].[tb_smnfi_solicitacao_saida_material_nota_fiscal_item]
                          WHERE [smnfi_idt] = @IdSaidaMaterialNotaFiscalItem";

            var result = await _db.Connection.QueryFirstOrDefaultAsync<SaidaMaterialNotaFiscalItemModel>(new CommandDefinition(
                    commandText: query,
                    parameters: new { IdSaidaMaterialNotaFiscalIte = idSaidaMaterialNotaFiscalItem },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );

            return result;
        }

        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalItemModel>>> ObterPorIdSolicitacaoSaidaMaterial(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<SaidaMaterialNotaFiscalItemModel>(
            new CommandDefinition(
                commandText: $@"  
                            SELECT
                                  smnfi.smnfi_idt as [id],
	                              smnfi.smnfi_idt as [IdSaidaMaterialNotaFiscalItem],
	                              smnfi.smnf_idt as [IdSolicitacaoSaidaMaterialNF],
	                              smnfi.smnfi_qtd as [Quantidade],
	                              smnfi.smnfi_unidade as [Unidade],
	                              smnfi.smnfi_patrimonio as [Patrimonio],
	                              smnfi.smnfi_des as [Descricao],
	                              smnfi.smnfi_valor_unitario as [ValorUnitario],
	                              smnfi.smnfi_tag_servico as [TagService],
	                              smnfi.smnfi_codigo as [Codigo],
								  smnfi.smnfi_flg_ativo as [Ativo],
								  smnfi.smnfi_dat_criacao as [DataCriacao],
								  smnfi.smnfi_dat_atualizacao as [DataAtualizacao]
						    FROM
							      tb_smnfi_saida_material_nota_fiscal_item smnfi
							WHERE 
							     smnfi.smnf_idt = @idSaidaMaterialNotaFiscal AND
							     smnfi.smnfi_flg_ativo = 1",
                    parameters: new
                    {
                        idSaidaMaterialNotaFiscal,
                    },
                    transaction: Transaction
               )
           );
            return result?.ToCollection();
        }
        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalItemModel>>> ObterItemMateriaParaAcaoPorId(int idSolicitacaoSaidaMaterialNf, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<SaidaMaterialNotaFiscalItemModel>(
             new CommandDefinition(
                 commandText: $@"SELECT 
						          smnfi.smnfi_idt as [id],
                                  smnfi.smnfi_idt as [IdSaidaMaterialNotaFiscalItem],
	                              smnfi.smnf_idt as [IdSolicitacaoSaidaMaterialNF],
	                              smnfi.smnfi_qtd as [Quantidade],
	                              smnfi.smnfi_unidade as [Unidade],
	                              smnfi.smnfi_patrimonio as [Patrimonio],
	                              smnfi.smnfi_des as [Descricao],
	                              smnfi.smnfi_valor_unitario as [ValorUnitario],
	                              smnfi.smnfi_tag_servico as [TagService],
	                              smnfi.smnfi_codigo as [Codigo],
								  smnfi.smnfi_flg_ativo as [Ativo],
								  smnfi.smnfi_dat_criacao as [DataCriacao],
								  smnfi.smnfi_dat_atualizacao as [DataAtualizacao]
						    FROM
							      tb_smnfi_saida_material_nota_fiscal_item smnfi
								WHERE 
								    smnfi.smnf_idt = @idSolicitacaoSaidaMaterialNf AND
									smnfi.smnfi_idt NOT IN ((
									SELECT 
									smnfai.smnfi_idt 
									FROM 
									tb_smnf_saida_material_nota_fiscal smnf 
									INNER JOIN 
									tb_smnfa_saida_material_nota_fiscal_acao smnfa 
									ON smnfa.smnf_idt = smnf.smnf_idt
									INNER JOIN 
									tb_smnfai_saida_material_nota_fiscal_acao_item smnfai
									ON  smnfai.smnfa_idt = smnfa.smnfa_idt
									WHERE
									smnfa.smnf_idt = @idSolicitacaoSaidaMaterialNf AND 
									smnfa.smtanf_idt  IN ({(int)SaidaMaterialNotaFiscalTipoAcao.RegistroRetorno},{(int)SaidaMaterialNotaFiscalTipoAcao.BaixaMaterialSemRetorno})))",

                     parameters: new
                     {
                         idSolicitacaoSaidaMaterialNf,
                     },
                     cancellationToken: cancellationToken,
                     transaction: Transaction
                )
            );
            return result?.ToCollection();
        }

        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalItemProrrogadoModel>>> ObterItensProrrogadosEAprovados(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<SaidaMaterialNotaFiscalItemProrrogadoModel>(
            new CommandDefinition(
                commandText: $@"
                               SELECT 
	                                COUNT(smnfi_idt) AS TotalAprovadores, smnfi_idt AS IdSaidaMaterialNotaFiscalItem
                                FROM 
	                                tb_scunfa_solicitacao_ciencia_usuario_nota_fiscal_aprovacao scunfa
                                INNER JOIN 
	                                tb_socnf_solicitacao_ciencia_nota_fiscal socnf ON socnf.socnf_idt = scunfa.socnf_idt
                                INNER JOIN
	                                tb_scitnf_solicitacao_ciencia_item_nota_fiscal scitnf ON  scitnf.socnf_idt = socnf.socnf_idt
                                WHERE 
	                                socnf.smnf_idt = @idSaidaMaterialNotaFiscal
                                AND 
	                                scunfa_dat_aprovacao IS NOT NULL
                                GROUP BY 
	                                smnfi_idt",
                    parameters: new
                    {
                        idSaidaMaterialNotaFiscal,
                    }
               )
           );
            return result?.ToCollection();
        }

        public async Task<TryException<Return>> DesativarItemMaterial(IEnumerable<int> smnfiIdt, CancellationToken cancellationToken)
        {
            var command = @"DELETE 
                                    tb_smnfi_saida_material_nota_fiscal_item
                               WHERE
                                    smnfi_idt in @smnfiIdt";

            await _db.Connection.ExecuteAsync(new CommandDefinition(
                    commandText: command,
                    parameters: new
                    {
                        smnfiIdt
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );

            return Return.Empty;
        }

        public async Task<TryException<Return>> Inserir(SaidaMaterialNotaFiscalItemModel itemSaidaMaterialNFModel, CancellationToken cancellationToken)
        {
            var command = @"MERGE INTO [dbo].tb_smnfi_saida_material_nota_fiscal_item AS Target
				USING
				(
					   VALUES
                           (@Id
						   ,@IdSolicitacaoSaidaMaterialNF
                           ,@Quantidade
                           ,@Unidade
                           ,@Codigo
                           ,@ValorUnitario
						   ,@TagService
                           ,@Patrimonio
                           ,@Descricao
						   ,@Ativo
                           ,GETDATE()
                           ,GETDATE())
				) AS Source 
			       ([smnfi_idt], [smnf_idt] ,[smnfi_qtd] ,[smnfi_unidade] ,[smnfi_codigo] ,[smnfi_valor_unitario] ,[smnfi_tag_servico],[smnfi_patrimonio],[smnfi_des],[smnfi_flg_ativo]
                    ,[smnfi_dat_criacao],[smnfi_dat_atualizacao])
					
				ON Target.[smnfi_idt] = Source.[smnfi_idt]

				--altera registros que existem no Target e no Source
				WHEN MATCHED THEN
					UPDATE SET 
                             [smnfi_qtd] = Source.[smnfi_qtd]
                            ,[smnfi_unidade] = Source.[smnfi_unidade]
                            ,[smnfi_codigo] = Source.[smnfi_codigo]
                            ,[smnfi_valor_unitario] = Source.[smnfi_valor_unitario]
                            ,[smnfi_tag_servico] = Source.[smnfi_tag_servico]                                      
                            ,[smnfi_patrimonio] = Source.[smnfi_patrimonio]         
                            ,[smnfi_des] = Source.[smnfi_des]                                                                                
                            ,[smnfi_flg_ativo] = Source.[smnfi_flg_ativo]                                                                                
                            ,[smnfi_dat_atualizacao] = Source.[smnfi_dat_atualizacao]                                                                                

				--inseri novos registros que não existem no target e existem no source
				WHEN NOT MATCHED BY TARGET THEN 
					INSERT 
                          ([smnf_idt] 
						  ,[smnfi_qtd] 
						  ,[smnfi_unidade] 
						  ,[smnfi_codigo] 
						  ,[smnfi_valor_unitario] 
						  ,[smnfi_tag_servico]
						  ,[smnfi_patrimonio]
						  ,[smnfi_des]
						  ,[smnfi_flg_ativo]
                          ,[smnfi_dat_criacao]
						  ,[smnfi_dat_atualizacao])
                        VALUES
                          ([smnf_idt] 
						  ,[smnfi_qtd] 
						  ,[smnfi_unidade] 
						  ,[smnfi_codigo] 
						  ,[smnfi_valor_unitario] 
						  ,[smnfi_tag_servico]
						  ,[smnfi_patrimonio]
						  ,[smnfi_des]
						  ,[smnfi_flg_ativo]
                          ,[smnfi_dat_criacao]
						  ,[smnfi_dat_atualizacao])
                OUTPUT
                    INSERTED.[smnfi_idt];";

                   await _db.Connection.QuerySingleAsync<int>(new CommandDefinition(
                    commandText: command,
                    parameters: itemSaidaMaterialNFModel,
                    transaction: Transaction,
                    cancellationToken: cancellationToken));

            return Return.Empty;
        }

        public async Task<TryException<SaidaMaterialNotaFiscalItemModel>> Atualizar(SaidaMaterialNotaFiscalItemModel saidaMaterialNotaFiscalItemModel)
        {
            var command = @"UPDATE [dbo].[tb_smnfi_saida_material_nota_fiscal_item]
                           SET [smnf_idt] = @IdSaidaMaterialNotaFiscal
                              ,[smnfi_qtd] = @Quantidade
                              ,[smnfi_unidade] = @Unidade
                              ,[smnfi_patrimonio] = @Patrimonio
                              ,[smnfi_descricao] = @Descricao
                              ,[smnfi_flg_ativo] = @Ativo
                              ,[smnfi_dat_atualização] = @DataAtualizacao
                         WHERE smnfi_idt = @Id";

            await _db.Connection.ExecuteAsync(command, saidaMaterialNotaFiscalItemModel);

            return saidaMaterialNotaFiscalItemModel;
        }

        public async Task<TryException<Return>> Excluir(int id)
        {
            var command = @"DELETE FROM [dbo].[tb_smnfi_saida_material_nota_fiscal_item] WHERE smnfi_idt = @Id";
            await _db.Connection.ExecuteAsync(command, new { id });
            return Return.Empty;
        }

        public async Task<TryException<int>> ObterQuantidadeItemComBaixa(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken)
        {
            return await _db.Connection.QueryFirstOrDefaultAsync<int>(new CommandDefinition(
                    commandText: $@"SELECT 
									COUNT(smnf.smnf_idt) as [Quantidade] 
									    FROM 
									tb_smnf_saida_material_nota_fiscal smnf 
									INNER JOIN 
									tb_smnfa_saida_material_nota_fiscal_acao smnfa 
									ON smnfa.smnf_idt = smnf.smnf_idt
									INNER JOIN 
									tb_smnfai_saida_material_nota_fiscal_acao_item smnfai
									ON  smnfai.smnfa_idt = smnfa.smnfa_idt
									    WHERE
									smnfa.smnf_idt = @idSaidaMaterialNotaFiscal AND 
									smnfa.smtanf_idt IN ({(int)SaidaMaterialTipoAcao.RegistroRetorno},{(int)SaidaMaterialTipoAcao.BaixaMaterialSemRetorno})",
                    parameters: new
                    {
                        idSaidaMaterialNotaFiscal
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                ));
        }

        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalItemModel>>> ObterItemMateriaParaRetornoPorId(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<SaidaMaterialNotaFiscalItemModel>(
            new CommandDefinition(
                commandText: $@"SELECT 
									    smnfi.[smnfi_idt] as IdSaidaMaterialNotaFiscalItem
									   ,smnfi.[smnfi_idt] as  IdSaidaMaterialNotaFiscal
									   ,smnfi.[smnfi_qtd] as Quantidade
									   ,smnfi.[smnfi_unidade] as Unidade
									   ,smnfi.[smnfi_patrimonio] as Patrimonio
									   ,smnfi.[smnfi_descricao] as Descricao
									   ,smnfi.[smnfi_dat_criacao] as DataCriacao
									   ,smnfi.[smnfi_flg_ativo] as Ativo
								FROM'
                                       tb_smnfi_saida_material_nota_fiscal_item smnfi 
								WHERE 
								    smnfi.smnf_idt = @idSaidaMaterialNotaFiscal AND
									smnfi.smnfi_idt NOT IN ((
									SELECT 
									smnfai.smnf_idt 
									FROM 
									tb_smnf_saida_material_nota_fiscal smnf 
									INNER JOIN 
									tb_smnfa_saida_material_nota_fiscal_acao smnfa 
									ON smnfa.smnf_idt = smnf.smnf_idt
									INNER JOIN 
									tb_smnfai_saida_material_nota_fiscal_acao_item smnfai
									ON  smnfai.smnfa_idt = smnfa.smnfa_idt
									WHERE
									smnfa.smnf_idt = @idSaidaMaterialNotaFiscal AND 
									smnfa.smtanf_idt  IN ({(int)SaidaMaterialTipoAcao.RegistroRetorno},{(int)SaidaMaterialTipoAcao.BaixaMaterialSemRetorno})))",
                    parameters: new
                    {
                        idSaidaMaterialNotaFiscal,
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
               ));
            return result?.ToCollection();
        }
    }
}
