using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using Dapper;
using System.Threading.Tasks;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial;
using System.Collections.Generic;
using System.Threading;
using System.Data.SqlTypes;
using System.Linq;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories.SolicitacaoSaidaMaterial
{
    internal class SolicitacaoSaidaMaterialRepository : Repository, ISolicitacaoSaidaMaterialRepository
    {
        public SolicitacaoSaidaMaterialRepository(IGDocsDatabase db, IUnitOfWork unitOfWork) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<SolicitacaoSaidaMaterialModel>> ObterPorId(int id, CancellationToken cancellationToken)
        {
            var query = @"SELECT
                            ssm.ssm_idt as [Id]
                           ,ssm.ssm_num as [Numero]
                           ,ssm.ssm_flg_retorno as [FlgRetorno]
                           ,case when ssm.ssm_flg_retorno = 1 then 'Com retorno' else 'Sem retorno' end as [TipoSaida]
                           ,ssm.ssm_guid_ad_responsavel as [GuidResponsavel]
                           ,ssm.ssm_nm_setor_responsavel as [SetorResponsavel]
                           ,ssm.ssm_des_origem as [Origem]
                           ,ssm.ssm_des_destino as [Destino]
                           ,ssm.ssm_dat_retorno as [Retorno]
                           ,ssm.ssm_des_motivo as [Motivo]
                           ,ssm.ssm_des_observacao as [Observacao]
                           ,ssm.bin_idt as [BinarioId]
                           ,ssm.stsm_idt as [StatusId]
                           ,stsm.stsm_des as [Status]
                           ,ssm.ssm_dat_cricao as [DataCriacao]
                           ,ssm.ssm_flg_ativo as [Ativo]
                           ,ssm.ssm_ret_parcial as [RetornoParcial]
                          FROM 
                            [dbo].[tb_ssm_solicitacao_saida_material] ssm inner join [dbo].[tb_stsm_status_saida_material] stsm
                                                                            on ssm.stsm_idt = stsm.stsm_idt
                          WHERE 
                            ssm_idt = @id";

            var result = await _db.Connection.QueryFirstOrDefaultAsync<SolicitacaoSaidaMaterialModel>(new CommandDefinition(
                    commandText: query,
                    parameters: new { id },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );

            return result;
        }

        public async Task<TryException<SolicitacaoSaidaMaterialModel>> Inserir(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken)
        {
            var command = @"MERGE INTO [dbo].[tb_ssm_solicitacao_saida_material] AS Target
				USING
				(
			      VALUES
                   (@Id, @Numero, @FlgRetorno, @GuidResponsavel, @SetorResponsavel, @Origem, @Destino, @Retorno, @Motivo, @Observacao, GETDATE(), 1,NULL, @StatusId, GETDATE(),@Autor, 0)
				) AS Source 

			  ([ssm_idt],[ssm_num] ,[ssm_flg_retorno] ,[ssm_guid_ad_responsavel] ,[ssm_nm_setor_responsavel] ,[ssm_des_origem] ,[ssm_des_destino] ,[ssm_dat_retorno]
               ,[ssm_des_motivo] ,[ssm_des_observacao] ,[ssm_dat_cricao] ,[ssm_flg_ativo] ,[bin_idt] ,[stsm_idt] ,[ssm_dat_atualizacao] ,[ssm_guid_ad_autor], [ssm_ret_parcial])
					
				ON Target.[ssm_idt] = Source.[ssm_idt]

				--altera registros que existem no Target e no Source
				WHEN MATCHED THEN
					UPDATE SET 
                             [ssm_flg_retorno] = Source.[ssm_flg_retorno]
                            ,[ssm_guid_ad_responsavel] = Source.[ssm_guid_ad_responsavel]
                            ,[ssm_des_origem] = Source.[ssm_des_origem]            
                            ,[ssm_nm_setor_responsavel] = Source.[ssm_nm_setor_responsavel]
                            ,[ssm_des_destino] = Source.[ssm_des_destino]
                            ,[ssm_dat_retorno] = Source.[ssm_dat_retorno]                                       
                            ,[ssm_des_motivo] = Source.[ssm_des_motivo]                                                                                
                            ,[ssm_des_observacao] = Source.[ssm_des_observacao]
							,[ssm_dat_cricao] = GETDATE()
                            ,[ssm_flg_ativo] = Source.[ssm_flg_ativo]
                            ,[bin_idt] = Source.[bin_idt]
                            ,[ssm_dat_atualizacao] = GETDATE()
                            ,[ssm_guid_ad_autor] = Source.[ssm_guid_ad_autor]
                            ,[ssm_ret_parcial] = Source.[ssm_ret_parcial]

				--inseri novos registros que não existem no target e existem no source
				WHEN NOT MATCHED BY TARGET THEN 
					INSERT 
                            ([ssm_num]
                           ,[ssm_flg_retorno]
                           ,[ssm_guid_ad_responsavel]
                           ,[ssm_nm_setor_responsavel]
                           ,[ssm_des_origem]
                           ,[ssm_des_destino]
                           ,[ssm_dat_retorno]
                           ,[ssm_des_motivo]
                           ,[ssm_des_observacao]
                           ,[ssm_dat_cricao]
                           ,[ssm_flg_ativo]
                           ,[bin_idt]
                           ,[stsm_idt]
                           ,[ssm_dat_atualizacao]
                           ,[ssm_guid_ad_autor]
                           ,[ssm_ret_parcial])
                        VALUES
                            ([ssm_num]
                           ,[ssm_flg_retorno]
                           ,[ssm_guid_ad_responsavel]
                           ,[ssm_nm_setor_responsavel]
                           ,[ssm_des_origem]
                           ,[ssm_des_destino]
                           ,[ssm_dat_retorno]
                           ,[ssm_des_motivo]
                           ,[ssm_des_observacao]
                           ,[ssm_dat_cricao]
                           ,[ssm_flg_ativo]
                           ,[bin_idt]
                           ,[stsm_idt]
                           ,[ssm_dat_atualizacao]
                           ,[ssm_guid_ad_autor]
                           ,[ssm_ret_parcial])
                OUTPUT
                    INSERTED.[ssm_idt];";

            var id = await _db.Connection.QuerySingleAsync<int>(new CommandDefinition(
                    commandText: command,
                    parameters: solicitacaoSaidaMaterialModel,
                    transaction: Transaction,
                    cancellationToken: cancellationToken));

            solicitacaoSaidaMaterialModel.DefinirMaterialId(id);
            return solicitacaoSaidaMaterialModel;
        }

        public async Task<TryException<SolicitacaoSaidaMaterialModel>> Atualizar(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken)
        {
            var command = @"UPDATE [dbo].[tb_ssm_solicitacao_saida_material]
                            SET [ssm_num] = @Numero
                                ,[ssm_flg_retorno] = @FlgRetorno
                                ,[ssm_guid_ad_responsavel] = @GuidResponsavel
                                ,[ssm_nm_setor_responsavel] = @SetorResponsavel
                                ,[ssm_des_origem] = @Origem
                                ,[ssm_des_destino] = @Destino
                                ,[ssm_dat_retorno] = @Retorno
                                ,[ssm_des_motivo] = @Motivo
                                ,[ssm_des_observacao] = @Observacao
                                ,[bin_idt] = @BinarioId
                                ,[ssm_dat_atualizacao] = GETDATE()
                            WHERE ssm_idt = @Id";

            await _db.Connection.ExecuteAsync(new CommandDefinition(
                    commandText: command,
                    parameters: solicitacaoSaidaMaterialModel,
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );

            return solicitacaoSaidaMaterialModel;
        }

        public async Task<TryException<Return>> AtualizarStatus(int solicitacaoSaidaMaterialId, SolicitacaoSaidaMaterialStatus novoStatus, CancellationToken cancellationToken, bool retornoParcial = false)
        {
            await _db.Connection.ExecuteAsync(
                new CommandDefinition(
                    commandText: @"
                        UPDATE 
                            [dbo].[tb_ssm_solicitacao_saida_material]
                        SET 
                            stsm_idt = @StatusId,
                            ssm_ret_parcial = @RetornoParcial,
                            ssm_dat_atualizacao = GETDATE()
                        WHERE 
                            ssm_idt = @Id",
                    transaction: Transaction,
                    cancellationToken: cancellationToken,
                    parameters: new
                    {
                        Id = solicitacaoSaidaMaterialId,
                        StatusId = novoStatus.ToInt32(),
                        RetornoParcial = retornoParcial
                    }
                )
            );

            return Return.Empty;
        }


        public async Task<TryException<Return>> AtualizarDataStatus(SolicitacaoCienciaModel solicitacaoCienciaModel, SolicitacaoSaidaMaterialStatus novoStatus, CancellationToken cancellationToken)
        {
            await _db.Connection.ExecuteAsync(
                new CommandDefinition(
                    commandText: @"
                        UPDATE 
                            [dbo].[tb_ssm_solicitacao_saida_material]
                        SET 
                            stsm_idt = @StatusId,
                            ssm_dat_retorno = @Prorrogacao,
                            ssm_dat_atualizacao = GETDATE()
                        WHERE 
                            ssm_idt = @Id",
                    transaction: Transaction,
                    cancellationToken: cancellationToken,
                    parameters: new
                    {
                        Id = solicitacaoCienciaModel.IdSolicitacaoSaidaMaterial,
                        Prorrogacao = solicitacaoCienciaModel.DataProrrogacao,
                        StatusId = novoStatus.ToInt32()
                    }
                )
            );

            return Return.Empty;
        }

        public async Task<TryException<Return>> RegistroHistoricoProrogacao(SolicitacaoCienciaModel solicitacaoCienciaModel, CancellationToken cancellationToken)
        {
            await _db.Connection.ExecuteAsync(
                new CommandDefinition(
                    commandText: @"
                            INSERT INTO [dbo].[tb_hsp_historico_solicitacao_prorogacao]
                           ([ssm_idt]
                           ,[ssma_idt]
                           ,[hsp_dat_de]
                           ,[hsp_dat_para]
                           )
                     VALUES
                           (@ssm_idt
                           ,@ssma_idt
                           ,@dataDe
                           ,@dataPara
                           );",
                    transaction: Transaction,
                    cancellationToken: cancellationToken,
                    parameters: new
                    {
                        ssm_idt = solicitacaoCienciaModel.IdSolicitacaoSaidaMaterial,
                        ssma_idt = solicitacaoCienciaModel.IdSolicitacaoSaidaMaterialAcao,
                        dataDe = solicitacaoCienciaModel.DataRetorno,
                        dataPara = solicitacaoCienciaModel.DataProrrogacao,
                    }
                )
            );

            return Return.Empty;
        }

        public async Task<TryException<Return>> Excluir(int id)
        {
            var command = @"UPDATE FROM [dbo].[tb_ssm_solicitacao_saida_material] SET ssm_flg_ativo = 0, ssm_dat_atualizacao = GETDATE() WHERE ssm_idt = @id";
            await _db.Connection.ExecuteAsync(command, new { id });
            return Return.Empty;
        }

        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialModel>>> Listar(SolicitacaoSaidaMaterialFilterModel filtro, CancellationToken cancellationToken)
        {
            var query = $@"   SELECT 
                                   ssm.ssm_idt AS Id
								  ,ssm.ssm_num AS Numero
								  ,ssm.ssm_flg_retorno AS FlgRetorno
                                  ,case when ssm.ssm_flg_retorno = 1 then 'Com Retorno' else 'Sem Retorno' end TipoSaida
								  ,ssm.ssm_guid_ad_responsavel AS GuidResponsavel
								  ,ssm.ssm_nm_setor_responsavel AS SetorResponsavel
								  ,ssm.ssm_des_origem AS Origem
								  ,ssm.ssm_des_destino AS Destino
								  ,ssm.ssm_dat_retorno AS Retorno
								  ,ssm.ssm_des_motivo AS Motivo
								  ,ssm.ssm_des_observacao AS Observacao
								  ,ssm.ssm_dat_cricao AS DataCriacao
								  ,ssm.ssm_flg_ativo AS Ativo
								  ,ssm.bin_idt AS BinarioId
								  ,ssm.stsm_idt AS StatusId
                                  ,stsm.stsm_des AS Status
                                  ,ssm.ssm_guid_ad_autor AS Autor
								  ,ssma.ssma_dat_acao as DataAcao
                                  ,ssm.ssm_ret_parcial as RetornoParcial
                                  ,[dbo].[fn_retorna_itens_solicitacao_saida_material](ssm.ssm_idt) AS HtmlItens
                              FROM 
								   dbo.tb_ssm_solicitacao_saida_material ssm
								inner join 
								   tb_stsm_status_saida_material stsm
									on ssm.stsm_idt = stsm.stsm_idt
                              LEFT join tb_ssma_solicitacao_saida_material_acao ssma on ssma.ssm_idt = ssm.ssm_idt  and ssma.smta_idt = 1
                              WHERE
                                   ssm.ssm_flg_ativo = 1
                                   AND (@id = 0 OR ssm.ssm_idt = @id)
                                   AND (@numero = 0 OR ssm.ssm_num = @numero)
                                   {MontarFiltroTiposDeSaida(filtro.TiposSaida)}
								   AND (CONVERT(DATE, ssm_dat_cricao) BETWEEN CONVERT(DATE, @dataInicio) AND CONVERT(DATE, @dataTermino))
                                   AND (@countAutores = 0 OR ssm.ssm_guid_ad_responsavel IN @autores)
                                   AND (@countStatus = 0 OR ssm.stsm_idt IN @status)
                                   AND (@vencido = 0 OR CONVERT(DATE, ssm.ssm_dat_retorno) < CONVERT(DATE, GETDATE()) AND ssm.stsm_idt NOT IN ({(int)SolicitacaoSaidaMaterialStatus.Cancelado}, {(int)SolicitacaoSaidaMaterialStatus.Concluido}))
	                               AND (
                                       @usuarioLogadoAd = '00000000-0000-0000-0000-000000000000'
                                       OR
		                                (@usuarioLogadoAd <> ssm.ssm_guid_ad_responsavel  )
		                                OR
		                               @usuarioLogadoAd = ssm.ssm_guid_ad_responsavel
	                                  )
                                   AND (@patrimonio = '' OR exists (SELECT top 1 1 FROM tb_ssmi_solicitacao_saida_material_item ssmi where ssmi.ssm_idt = ssm.ssm_idt and ssmi.ssm_num_patrimonio = @patrimonio))
                              ORDER BY
                                    {filtro.Ordenacao.OrderBy}";

            var result = await _db.Connection.QueryAsync<SolicitacaoSaidaMaterialModel>(
                new CommandDefinition(
                    commandText: query,
                    parameters: new
                    {
                        id = filtro.Id,
                        numero = filtro.Numero,
                        autores = filtro.Autores,
                        dataInicio = filtro.DataInicio.GetValueOrDefault(SqlDateTime.MinValue.Value),
                        dataTermino = filtro.DataTermino.GetValueOrDefault(SqlDateTime.MaxValue.Value),
                        vencido = filtro.Vencido ? 1 : 0,
                        status = filtro.Status,
                        countAutores = filtro.Autores.Count(),
                        countStatus = filtro.Status.Count(),
                        usuarioLogadoAd = filtro.UsuarioLogadoAd,
                        patrimonio = filtro.Patrimonio == null ? "" : filtro.Patrimonio,
                        statusEmConstrucao = SolicitacaoSaidaMaterialStatus.EmConstrucao.ToInt32()
                    },
                    cancellationToken: cancellationToken
                )
            );

            return result?.ToCollection();
        }

        private string MontarFiltroTiposDeSaida(IEnumerable<int> tiposSaida)
        {
            if (!tiposSaida.Any() ||
                (tiposSaida.Contains(0) && tiposSaida.Contains(1)))
                return "";

            return $"AND ssm.ssm_flg_retorno = {(tiposSaida.Contains(1) ? 1 : 0)}";
        }

        public async Task<TryException<IEnumerable<DocumentoFI347StatusModel>>> ListarStatusMaterial(CancellationToken cancellationToken)
            => (await _db.Connection.QueryAsync<DocumentoFI347StatusModel>(
                new CommandDefinition(
                    commandText: $@"SELECT
	                                   stsm.[stsm_idt] [Id],
                                       stsm.[stsm_des] [Descricao]
                                    FROM 
	                                    [dbo].[tb_stsm_status_saida_material] stsm
                                    WHERE
	                                    stsm.[stsm_flg_ativo] = 1
									ORDER BY
									Descricao ASC",
                    cancellationToken: cancellationToken
                ))).ToCollection();

        public async Task<TryException<SolicitacaoSaidaMaterialModel>> ObterMaterialComItensPorId(int id, CancellationToken cancellationToken)
        {
            var listarMaterial = new Dictionary<int, SolicitacaoSaidaMaterialModel>();

            var result = await _db.Connection.QueryAsync<SolicitacaoSaidaMaterialModel, SolicitacaoSaidaMaterialItemModel, SolicitacaoSaidaMaterialModel>(
                   new CommandDefinition(
                       commandText: @"SELECT 
                                           ssm.ssm_idt as [Id]
										   ,ssm.ssm_num as [Numero]
										   ,ssm.ssm_flg_retorno as [FlgRetorno]
										   ,case when ssm.ssm_flg_retorno = 1 then 'Com retorno' else 'Sem retorno' end as [TipoSaida]
										   ,ssm.ssm_guid_ad_responsavel as [GuidResponsavel]
                                           ,ssm.ssm_guid_ad_autor as [Autor]
										   ,ssm.ssm_nm_setor_responsavel as [SetorResponsavel]
										   ,ssm.ssm_des_origem as [Origem]
										   ,ssm.ssm_des_destino as [Destino]
										   ,ssm.ssm_dat_retorno as [Retorno]
										   ,ssm.ssm_des_motivo as [Motivo]
										   ,ssm.ssm_des_observacao as [Observacao]
										   ,ssm.bin_idt as [BinarioId]
										   ,ssm.stsm_idt as [StatusId]
										   ,stsm.stsm_des as [Status]
										   ,ssm.ssm_dat_cricao as [DataCriacao]
										   ,ssm.ssm_flg_ativo as [Ativo]
										   ,ssm.ssm_des_observacao as [Observacao]
										   ,ssm.ssm_des_motivo as [Motivo],

	                                        ssmi.ssmi_idt as [IdSolicitacaoSaidaMaterialItem],
	                                        ssm.ssm_idt as [IdSolicitacaoSaidaMaterial],
	                                        ssmi.ssmi_qtd_item as [Quantidade],
	                                        ssmi.ssmi_unidade as [Unidade],
	                                        ssmi.ssm_num_patrimonio as [Patrimonio],
	                                        ssmi.ssmi_descricao as [Descricao],
											ssmi.ssmi_flg_ativo as [Ativo],
											ssmi.ssmi_dat_criacao as [DataCriacao]
                                        FROM 
	                                        tb_ssm_solicitacao_saida_material ssm 
                                            INNER JOIN tb_ssmi_solicitacao_saida_material_item ssmi on ssmi.ssm_idt = ssm.ssm_idt
											inner join [dbo].[tb_stsm_status_saida_material] stsm on ssm.stsm_idt = stsm.stsm_idt
                                        WHERE
                                      ssm.ssm_idt = @id
	                                    and ssm.ssm_flg_ativo = 1
	                                    and ssmi.ssmi_flg_ativo = 1",

                            parameters: new
                            {
                                id 
                            },
                        cancellationToken: cancellationToken
                    ),
                    splitOn: "Id,IdSolicitacaoSaidaMaterialItem",
                    map: (materialMap, itemMap) =>
                    {
                        if (!listarMaterial.TryGetValue(materialMap.Id, out var material))
                        {
                            material = materialMap;
                            listarMaterial.Add(material.Id, material);
                        }

                        material.ItemMaterial.Add(itemMap);

                        return material;
                    }
                );

            return result.FirstOrDefault();
        }
    }
}
