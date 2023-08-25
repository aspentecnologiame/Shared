using Dapper;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.Data.Core.Repositories;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Infra.CrossCutting.Models;
using System.Collections.Generic;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System.Data.SqlTypes;
using System.Linq;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories.SaidaMaterialNotaFiscal.Service
{
    internal class SaidaMaterialNotaFiscalRepository : Repository, ISaidaMaterialNotaFiscalRepository
    {
        public SaidaMaterialNotaFiscalRepository(IGDocsDatabase db, IUnitOfWork unitOfWork) : base(db, unitOfWork)
        {

        }

        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalModel>>> ConsultaMaterialNotaFiscalPorFiltro(SaidaMaterialNotaFiscalFilterModel filtroConsulta, CancellationToken cancellationToken, bool buscarNfCancelada = false)
        {
            var query = $@"SELECT 
                                   smnf.smnf_idt AS Id
								  ,smnf.smnf_num AS Numero
								  ,smnf.smnf_flg_retorno AS FlgRetorno
                                  ,case when smnf.smnf_flg_retorno = 1 then 'Com Retorno' else 'Sem Retorno' end TipoSaida
                                  ,smnf.smnf_guid_ad_autor AS GuidAutor
                                  ,smnf.smnf_guid_ad_autor AS GuidResponsavel
								  ,smnf.smnf_setor AS SetorResponsavel
								  ,smnf.smnf_origem AS Origem
								  ,smnf.smnf_destino AS Destino
								  ,smnf.smnf_dat_retorno AS Retorno
								  ,smnf.smnf_motivo AS Motivo
								  ,smnf.smnf_dat_criacao AS DataCriacao
								  ,smnf.smnf_flg_ativo AS Ativo
								  ,stsmnf.stsmnf_idt AS StatusId
                                  ,stsmnf.stsmnf_des AS Status
                                  ,smnfa.smnfa_dat_acao as DataAcao
                                  ,smnf.smnf_ret_parcial as RetornoParcial
                                  ,[dbo].[fn_retorna_itens_solicitacao_saida_material_nota_fiscal](smnf.smnf_idt) AS HtmlItens
                              {this.ComplementarQuery(filtroConsulta, buscarNfCancelada)}";
            
            var result = await _db.Connection.QueryAsync<SaidaMaterialNotaFiscalModel>(
                new CommandDefinition(
                    commandText: query,
                    parameters: new
                    {
                        idFiltro = filtroConsulta.Id,
                        numeroFiltro = filtroConsulta.Numero,
                        autoresFiltro = filtroConsulta.Autores,
                        dataInicioFiltro = filtroConsulta.DataInicio.GetValueOrDefault(SqlDateTime.MinValue.Value),
                        dataTerminoFiltro = filtroConsulta.DataTermino.GetValueOrDefault(SqlDateTime.MaxValue.Value),
                        vencidoFiltro = filtroConsulta.Vencido ? 1 : 0,
                        statusFiltro = filtroConsulta.Status,
                        countAutoresFiltro = filtroConsulta.Autores.Count(),
                        countStatusFiltro = filtroConsulta.Status.Count(),
                        usuarioLogadoAdFiltro = filtroConsulta.UsuarioLogadoAd,
                        statusEmConstrucaoFiltro = SolicitacaoSaidaMaterialStatus.EmConstrucao.ToInt32()
                    },
                    cancellationToken: cancellationToken
                )
            );

            return result?.ToCollection();
        }

        private string ComplementarQuery(SaidaMaterialNotaFiscalFilterModel filtroConsulta, bool buscarNfCancelada)
        {
            if (buscarNfCancelada)
            {
                return $@"FROM 
								   dbo.tb_smnf_saida_material_nota_fiscal smnf
	                                INNER JOIN tb_stsmnf_status_saida_material_nota_fiscal stsmnf on smnf.stsmnf_idt = stsmnf.stsmnf_idt
                                    LEFT JOIN tb_smnfa_saida_material_nota_fiscal_acao smnfa on  smnfa.smnf_idt = smnf.smnf_idt and smnfa.smtanf_idt = 1
                                    LEFT JOIN tb_smdnf_saida_material_documento_nota_fiscal smdnf on smdnf.smnf_idt = smnf.smnf_idt
                              WHERE
                                   smnf.smnf_flg_ativo = 1
                                   AND (@idFiltro = 0 OR smnf.smnf_idt = @idFiltro)
                                   AND (@numeroFiltro = 0 OR (smdnf.smnf_numero_documento = @numeroFiltro AND smdnf.tdnf_idt = 1))
                                   {MontarFiltroTiposDeSaidaMaterialNF(filtroConsulta.TiposSaida)}
								   AND (CONVERT(DATE, smnf_dat_criacao) BETWEEN CONVERT(DATE, @dataInicioFiltro) AND CONVERT(DATE, @dataTerminoFiltro))
                                   AND (@countAutoresFiltro = 0 OR smnf.smnf_guid_ad_autor IN @autoresFiltro)
                                   AND (@countStatusFiltro = 0 OR stsmnf.stsmnf_idt IN @statusFiltro)
                                   AND (@vencidoFiltro = 0 OR CONVERT(DATE, smnf.smnf_dat_retorno) < CONVERT(DATE, GETDATE()) AND smnf.stsmnf_idt NOT IN ({(int)SaidaMaterialNotaFiscalStatus.Cancelada}, {(int)SaidaMaterialNotaFiscalStatus.Concluida}))
	                               AND (
                                       @usuarioLogadoAdFiltro = '00000000-0000-0000-0000-000000000000'
                                       OR
		                                (@usuarioLogadoAdFiltro <> smnf.smnf_guid_ad_autor  )
		                                OR
		                               @usuarioLogadoAdFiltro = smnf.smnf_guid_ad_autor
	                                  )
                                   ORDER BY
                                    {filtroConsulta.ExibirNaOrdem.OrderBy}";
            }
                
            return $@"FROM 
								   dbo.tb_smnf_saida_material_nota_fiscal smnf
	                                INNER JOIN tb_stsmnf_status_saida_material_nota_fiscal stsmnf on smnf.stsmnf_idt = stsmnf.stsmnf_idt
                                    LEFT JOIN tb_smnfa_saida_material_nota_fiscal_acao smnfa on  smnfa.smnf_idt = smnf.smnf_idt and smnfa.smtanf_idt = 1
                              WHERE
                                   smnf.smnf_flg_ativo = 1
                                   AND (@idFiltro = 0 OR smnf.smnf_idt = @idFiltro)
                                   AND (@numeroFiltro = 0 OR smnf.smnf_num = @numeroFiltro)
                                   {MontarFiltroTiposDeSaidaMaterialNF(filtroConsulta.TiposSaida)}
								   AND (CONVERT(DATE, smnf_dat_criacao) BETWEEN CONVERT(DATE, @dataInicioFiltro) AND CONVERT(DATE, @dataTerminoFiltro))
                                   AND (@countAutoresFiltro = 0 OR smnf.smnf_guid_ad_autor IN @autoresFiltro)
                                   AND (@countStatusFiltro = 0 OR stsmnf.stsmnf_idt IN @statusFiltro)
                                   AND (@vencidoFiltro = 0 OR CONVERT(DATE, smnf.smnf_dat_retorno) < CONVERT(DATE, GETDATE()) AND smnf.stsmnf_idt NOT IN ({(int)SaidaMaterialNotaFiscalStatus.Cancelada}, {(int)SaidaMaterialNotaFiscalStatus.Concluida}))
	                               AND (
                                       @usuarioLogadoAdFiltro = '00000000-0000-0000-0000-000000000000'
                                       OR
		                                (@usuarioLogadoAdFiltro <> smnf.smnf_guid_ad_autor  )
		                                OR
		                               @usuarioLogadoAdFiltro = smnf.smnf_guid_ad_autor
	                                  )
                                   ORDER BY
                                    {filtroConsulta.ExibirNaOrdem.OrderBy}";
        }

        private string MontarFiltroTiposDeSaidaMaterialNF(IEnumerable<int> tiposSaida)
        {
            if (!tiposSaida.Any() ||
                (tiposSaida.Contains(0) && tiposSaida.Contains(1)))
                return "";

            return $"AND smnf.smnf_flg_retorno = {(tiposSaida.Contains(1) ? 1 : 0)}";
        }

        public async Task<TryException<SaidaMaterialNotaFiscalModel>> Inserir(SaidaMaterialNotaFiscalModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken)
        {
          
            var command = @"MERGE INTO [dbo].[tb_smnf_saida_material_nota_fiscal] AS Target
				USING
				(
			      VALUES
                    (@Id, @Numero, @FlgRetorno, @GuidAutor, @SetorResponsavel, @Origem, @Destino, @Retorno,
				    @Saida ,@Volume, @Peso, @Transportador , @CodigoTotvs, @ModalidadeFrete, @NaturezaOperacional, @Documento, 
				    @Motivo ,GETDATE(), 1, @StatusId, GETDATE(), 0)
				) AS Source 
				    ([smnf_idt],[smnf_num] ,[smnf_flg_retorno] ,[smnf_guid_ad_autor] ,[smnf_setor] ,[smnf_origem] ,[smnf_destino] ,[smnf_dat_retorno] 
				    ,[smnf_dat_saida] ,[smnf_volume],[smnf_peso],[smnf_transportador],[smnf_codigo_totvs] ,[mfnf_idt] ,[tnnf_idt] ,[smnf_documento]
                    ,[smnf_motivo], [smnf_dat_criacao] ,[smnf_flg_ativo]  ,[stsm_idt] ,[smnf_dat_atualizacao], [smnf_ret_parcial])

				ON Target.[smnf_idt] = Source.[smnf_idt]

				--altera registros que existem no Target e no Source
				WHEN MATCHED THEN
					UPDATE SET 
                             [smnf_flg_retorno] = Source.[smnf_flg_retorno]
                            ,[smnf_guid_ad_autor] = Source.[smnf_guid_ad_autor]
                            ,[smnf_origem] = Source.[smnf_origem]            
                            ,[smnf_setor] = Source.[smnf_setor]
                            ,[smnf_destino] = Source.[smnf_destino]
                            ,[smnf_dat_retorno] = Source.[smnf_dat_retorno]                                       
                            ,[smnf_motivo] = Source.[smnf_motivo]
                            ,[smnf_dat_saida] = Source.[smnf_dat_saida]
                            ,[smnf_peso] = Source.[smnf_peso]
                            ,[smnf_volume] = Source.[smnf_volume]
                            ,[smnf_codigo_totvs] = Source.[smnf_codigo_totvs]
                            ,[smnf_documento] = Source.[smnf_documento]
						    ,[smnf_transportador] = Source.[smnf_transportador]
						    ,[mfnf_idt] = Source.[mfnf_idt]
						    ,[tnnf_idt] = Source.[tnnf_idt]
                            ,[smnf_flg_ativo] = Source.[smnf_flg_ativo]
                            ,[smnf_dat_atualizacao] = GETDATE()

				--inseri novos registros que não existem no target e existem no source
				WHEN NOT MATCHED BY TARGET THEN 
					INSERT 
                            ([smnf_num] ,[smnf_flg_retorno] ,[smnf_setor] ,[smnf_guid_ad_autor] ,[smnf_origem] ,[smnf_destino] ,[smnf_dat_retorno] ,[smnf_dat_saida]
						    ,[smnf_peso] ,[smnf_volume] ,[smnf_codigo_totvs] ,[smnf_documento] ,[smnf_motivo] ,[smnf_dat_criacao] ,[smnf_flg_ativo] ,[smnf_transportador]
					        ,[mfnf_idt] ,[tnnf_idt] ,[smnf_dat_atualizacao] ,[stsmnf_idt], [smnf_ret_parcial])
                        VALUES
                            ([smnf_num] ,[smnf_flg_retorno] ,[smnf_setor] ,[smnf_guid_ad_autor] ,[smnf_origem] ,[smnf_destino] ,[smnf_dat_retorno] ,[smnf_dat_saida]
						   ,[smnf_peso] ,[smnf_volume] ,[smnf_codigo_totvs] ,[smnf_documento] ,[smnf_motivo] ,[smnf_dat_criacao] ,[smnf_flg_ativo] ,[smnf_transportador]
					  	   ,[mfnf_idt]  ,[tnnf_idt] ,[smnf_dat_atualizacao] ,1, [smnf_ret_parcial])
                OUTPUT
                    INSERTED.[smnf_idt];";

            var id = await _db.Connection.QuerySingleAsync<int>(new CommandDefinition(
                    commandText: command,
                    parameters: solicitacaoSaidaMaterialModel,
                    transaction: Transaction,
                    cancellationToken: cancellationToken));

            solicitacaoSaidaMaterialModel.DefinirID(id);
            return solicitacaoSaidaMaterialModel;

        }


        public async Task<TryException<SaidaMaterialNotaFiscalModel>> InserirFornecedor(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, CancellationToken cancellationToken)
        {

            saidaMaterialNotaFiscalModel.Fornecedor.DefinirSolicitacaoMaterialNfId(saidaMaterialNotaFiscalModel.Id);
            var command = @"MERGE INTO [dbo].[tb_fnf_fornecedor_nota_fiscal] AS Target
				USING
				(
			      VALUES
                   (@Id, @SolicitacaoMaterialNfId, @Endereco, @Bairro, @Cep, @Cidade, @Estado , @Ativo ,GETDATE() ,GETDATE())
				) AS Source 
				 ([fnf_idt], [smnf_idt], [fnf_endereco],[fnf_bairro] ,[fnf_cep] ,[fnf_cidade] ,[fnf_estado] ,[socnf_flg_ativo] ,[socnf_dat_criacao] ,[socnf_dat_atualizacao])

				ON Target.[fnf_idt] = Source.[fnf_idt]

				--altera registros que existem no Target e no Source
				WHEN MATCHED THEN
					UPDATE SET 
                             [fnf_endereco] = Source.[fnf_endereco]
                            ,[fnf_bairro] = Source.[fnf_bairro]
                            ,[fnf_cep] = Source.[fnf_cep]            
                            ,[fnf_cidade] = Source.[fnf_cidade]
                            ,[fnf_estado] = Source.[fnf_estado]
                            ,[socnf_flg_ativo] = Source.[socnf_flg_ativo]                                       
                            ,[socnf_dat_atualizacao] = Source.[socnf_dat_atualizacao]

				--inseri novos registros que não existem no target e existem no source
				WHEN NOT MATCHED BY TARGET THEN 
					INSERT 
                            ([smnf_idt]
                           ,[fnf_endereco]
                           ,[fnf_bairro]
                           ,[fnf_cep]
                           ,[fnf_cidade]
                           ,[fnf_estado]
                           ,[socnf_flg_ativo]
                           ,[socnf_dat_criacao]
                           ,[socnf_dat_atualizacao])
                        VALUES
                          ([smnf_idt]
                           ,[fnf_endereco]
                           ,[fnf_bairro]
                           ,[fnf_cep]
                           ,[fnf_cidade]
                           ,[fnf_estado]
                           ,[socnf_flg_ativo]
                           ,[socnf_dat_criacao]
                           ,[socnf_dat_atualizacao])
                OUTPUT
                    INSERTED.[fnf_idt];";

            var id = await _db.Connection.QuerySingleAsync<int>(new CommandDefinition(
            commandText: command,
                    parameters: saidaMaterialNotaFiscalModel.Fornecedor,
                    transaction: Transaction,
                    cancellationToken: cancellationToken));

            saidaMaterialNotaFiscalModel.Fornecedor.DefinirId(id);
            return saidaMaterialNotaFiscalModel;
        }

        public async Task<TryException<IEnumerable<DropDownModel>>> ListarModalidadeFrete(CancellationToken cancellationToken) => (
            await _db.Connection.QueryAsync<DropDownModel>(
                new CommandDefinition(
                    commandText: $@"SELECT
	                                   mfnf.[mfnf_idt] [Id],
                                       mfnf.[mfnf_des] [Descricao]
                                    FROM 
	                                    [dbo].tb_mfnf_modalidade_frete_nota_fiscal mfnf
                                    WHERE
	                                    mfnf.[mfnf_flg_ativo] = 1
									ORDER BY
									Descricao ASC",
                    cancellationToken: cancellationToken
                ))).ToCollection();


        public async Task<TryException<IEnumerable<DropDownModel>>> ListarStatusMaterial(CancellationToken cancellationToken) => (
            await _db.Connection.QueryAsync<DropDownModel>(
                new CommandDefinition(
                    commandText: $@"SELECT
	                                   stsmnf.stsmnf_idt [Id],
                                       stsmnf.stsmnf_des [Descricao]
                                    FROM 
	                                    [dbo].tb_stsmnf_status_saida_material_nota_fiscal stsmnf
                                    WHERE
	                                    stsmnf.stsmnf_flg_ativo = 1
									ORDER BY
									Descricao ASC",
                    cancellationToken: cancellationToken
                ))).ToCollection();

        public async Task<TryException<IEnumerable<DropDownModel>>> ListarNatureza(CancellationToken cancellationToken) => (
            await _db.Connection.QueryAsync<DropDownModel>(
                new CommandDefinition(
                    commandText: $@"SELECT
	                                   tnnf.[tnnf_idt] [Id],
                                       tnnf.[tnnf_des] [Descricao]
                                    FROM 
	                                    [dbo].tb_tnnf_tipo_natureza_nota_fiscal tnnf
                                    WHERE
	                                    tnnf.[tnnf_flg_ativo] = 1
									ORDER BY
									Descricao ASC",
                    cancellationToken: cancellationToken
                ))).ToCollection();

        public async Task<TryException<SaidaMaterialNotaFiscalModel>> ObterMaterialNotaFiscalPorId(int id, CancellationToken cancellationToken)
        {
            var listarMaterialNf = new Dictionary<int, SaidaMaterialNotaFiscalModel>();
            var result = await _db.Connection.QueryAsync<SaidaMaterialNotaFiscalModel, SaidaMaterialNotaFiscalItemModel, Fornecedor, SaidaMaterialNotaFiscalModel>(
                   new CommandDefinition(
                       commandText: @"SELECT 
                                            smnf.smnf_idt as [Id]
										   ,smnf.smnf_num as [Numero]
										   ,smnf.smnf_flg_retorno as [FlgRetorno]
										   ,case when smnf.smnf_flg_retorno = 1 then 'Com retorno' else 'Sem retorno' end as [TipoSaida]
                                           ,smnf.smnf_guid_ad_autor as [GuidAutor]
										   ,smnf.smnf_setor as [SetorResponsavel]
										   ,smnf.smnf_origem as [Origem]
										   ,smnf.smnf_destino as [Destino]
										   ,smnf.smnf_dat_retorno as [Retorno]
										   ,smnf.smnf_motivo as [Motivo]
										   ,smnf.smnf_documento as [Documento]
										   ,smnf.smnf_codigo_totvs as [CodigoTotvs]
										   ,smnf.smnf_transportador as [Transportador]
										   ,smnf.smnf_dat_saida as [Saida]
										   ,smnf.smnf_peso as [Peso]
										   ,smnf.smnf_volume as [Volume]
										   ,smnf.mfnf_idt as [ModalidadeFrete]
										   ,smnf.tnnf_idt as [NaturezaOperacional]
										   ,tnnf.tnnf_des as [NaturezaOperacionalDesc]
										   ,mfnf.mfnf_des as [ModalidadeFreteDesc]
										   
										   ,stsmnf.stsmnf_idt AS StatusId
 										   ,stsmnf.stsmnf_des AS Status
										   ,smnf.smnf_dat_criacao as [DataCriacao]
										   ,smnf.smnf_flg_ativo as [Ativo],
										   	
											smnfi.smnfi_idt as [IdSaidaMaterialItem],
	                                        smnfi.smnfi_idt as [id],
	                                        smnf.smnf_idt as [IdSolicitacaoSaidaMaterialNF],
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

											,fnf.fnf_idt as idFornecedorNf
											,fnf.fnf_idt as id
											,fnf.smnf_idt as SolicitacaoMaterialNfId
											,fnf.fnf_endereco as Endereco
											,fnf.fnf_bairro as Bairro
											,fnf.fnf_cep as Cep
											,fnf.fnf_cidade as Cidade
											,fnf.fnf_estado as Estado
											,fnf.socnf_flg_ativo as Ativo
                                            ,smnf.smnf_ret_parcial as RetornoParcial
                                        FROM 
	                                        tb_smnf_saida_material_nota_fiscal smnf 
                                            INNER JOIN tb_smnfi_saida_material_nota_fiscal_item smnfi on smnfi.smnf_idt = smnf.smnf_idt
											INNER JOIN tb_stsmnf_status_saida_material_nota_fiscal stsmnf on smnf.stsmnf_idt = stsmnf.stsmnf_idt
											INNER JOIN tb_tnnf_tipo_natureza_nota_fiscal tnnf on tnnf.tnnf_idt = smnf.tnnf_idt
											INNER JOIN tb_mfnf_modalidade_frete_nota_fiscal mfnf on mfnf.mfnf_idt = smnf.mfnf_idt
											LEFT JOIN tb_fnf_fornecedor_nota_fiscal fnf on fnf.smnf_idt = smnf.smnf_idt
                                        WHERE
                                        smnf.smnf_idt = @id
	                                    AND smnf.smnf_flg_ativo = 1
	                                    AND smnfi.smnfi_flg_ativo = 1",
                        parameters: new { id },
                        cancellationToken: cancellationToken,
                        transaction: Transaction
                    ),
                    splitOn: "Id,IdSaidaMaterialItem,idFornecedorNf",
                    map: (materialMap, itemMap,fornecedorMap) =>
                    {
                        if (!listarMaterialNf.TryGetValue(materialMap.Id, out var material))
                        {
                            material = materialMap;
                            listarMaterialNf.Add(material.Id, material);
                        }

                        if (material.Fornecedor == null && fornecedorMap.ExisteFornecedor())
                        {
                            material.Fornecedor = new Fornecedor();
                            material.Fornecedor = fornecedorMap;
                        }

                        material.ItemMaterialNf.Add(itemMap);

                        return material;
                    }
                );

            return result.FirstOrDefault();
        } 

        public async Task<TryException<Return>> AtualizarStatusPendenteSaidaEhNumero(SaidaMaterialArquivoModel saidaMaterialArquivoModel, CancellationToken cancellationToken)
        {
            
            await _db.Connection.ExecuteAsync(
                   new CommandDefinition(
                       commandText: @"
                                UPDATE tb_smnf_saida_material_nota_fiscal   
                                    SET
                                    smnf_num = @Numero,
                                    stsmnf_idt = @Status
                                WHERE
                                    smnf_idt = @Id",
                       transaction: Transaction,
                       cancellationToken: cancellationToken,
                       parameters: new
                       {
                           Id = saidaMaterialArquivoModel.SaidaMaterialNfId,
                           Numero = saidaMaterialArquivoModel.Numero,
                           Status = SaidaMaterialNotaFiscalStatus.PendenteSaida.ToInt32()
                        }
                   ));

            
            return Return.Empty;


        }

        public async Task<TryException<Return>> AtualizarStatus(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, SaidaMaterialNotaFiscalStatus status, CancellationToken cancellationToken, bool retornoParcial = false)
        {
            await _db.Connection.ExecuteAsync(
               new CommandDefinition(
                   commandText: @"
                                UPDATE tb_smnf_saida_material_nota_fiscal   
                                    SET
                                    stsmnf_idt = @Status,
                                    smnf_ret_parcial = @RetornoParcial
                                WHERE
                                    smnf_idt = @Id",
                   transaction: Transaction,
                   cancellationToken: cancellationToken,
                   parameters: new
                   {
                       Id = saidaMaterialNotaFiscalModel.Id,
                       Status = status.ToInt32(),
                       RetornoParcial = retornoParcial
                   }
               ));


            return Return.Empty;
        }



        public async Task<TryException<Return>> AtualizarPorIdStatus(int saidaMaterialNotaFiscal, SaidaMaterialNotaFiscalStatus status, CancellationToken cancellationToken, bool retornoParcial = false)
        {
            await _db.Connection.ExecuteAsync(
               new CommandDefinition(
                   commandText: @"
                                UPDATE tb_smnf_saida_material_nota_fiscal   
                                    SET
                                    stsmnf_idt = @Status,
                                    smnf_ret_parcial = @RetornoParcial
                                WHERE
                                    smnf_idt = @Id",
                   transaction: Transaction,
                   cancellationToken: cancellationToken,
                   parameters: new
                   {
                       Id = saidaMaterialNotaFiscal,
                       Status = status.ToInt32(),
                       RetornoParcial = retornoParcial
                   }
               ));


            return Return.Empty;
        }

        public async Task<TryException<Return>> AtualizarDataStatus(SaidaMaterialNotaFiscalCienciaModel solicitacaoCienciaNfModel, SaidaMaterialNotaFiscalStatus novoStatus, CancellationToken cancellationToken)
        {
            await _db.Connection.ExecuteAsync(
                new CommandDefinition(
                    commandText: @"
                        UPDATE 
                            [dbo].[tb_smnf_saida_material_nota_fiscal]
                        SET 
                            stsmnf_idt = @StatusId,
                            smnf_dat_retorno = @Prorrogacao,
                            smnf_dat_atualizacao = GETDATE()
                        WHERE 
                            smnf_idt = @Id",
                    transaction: Transaction,
                    cancellationToken: cancellationToken,
                    parameters: new
                    {
                        Id = solicitacaoCienciaNfModel.IdSaidaMaterialNotaFiscal,
                        Prorrogacao = solicitacaoCienciaNfModel.DataProrrogacaoNF,
                        StatusId = novoStatus.ToInt32()
                    }
                )
            );

            return Return.Empty;
        }


        public async Task<TryException<Return>> RegistroHistoricoProrogacao(SaidaMaterialNotaFiscalCienciaModel solicitacaoCienciaModel, CancellationToken cancellationToken)
        {
            await _db.Connection.ExecuteAsync(
                new CommandDefinition(
                    commandText: @"
                    INSERT INTO
                    [dbo].[tb_hspnf_historico_solicitacao_prorogacao_nota_fiscal]
                           ([smnf_idt]
                           ,[smnfa_idt]
                           ,[hspnf_dat_de]
                           ,[hspnf_dat_para]
                           )
                     VALUES
                           (@IdMaterialSaidaNf
                           ,@IdMatetialAcaoNf
                           ,@dataDeNf
                           ,@dataParaNf
                           );",
                    transaction: Transaction,
                    cancellationToken: cancellationToken,
                    parameters: new
                    {
                        IdMaterialSaidaNf = solicitacaoCienciaModel.IdSaidaMaterialNotaFiscal,
                        IdMatetialAcaoNf = solicitacaoCienciaModel.IdSaidaMaterialNotaFiscalAcao,
                        dataDeNf = solicitacaoCienciaModel.DataRetorno,
                        dataParaNf = solicitacaoCienciaModel.DataProrrogacaoNF,
                    }
                )
            );

            return Return.Empty;
        }
            public async Task<TryException<Return>> InserirMotivoCancelar(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, string Motivo, CancellationToken cancellationToken)
        {
            await _db.Connection.ExecuteAsync(
               new CommandDefinition(
                   commandText: @"
                         INSERT INTO 
                         [dbo].[tb_csmnf_cancelar_saida_material_nota_fiscal]
                               ([smnf_idt]
                               ,[csmnf_motivo]
                               ,[csmnf_dat_criacao])
                         VALUES
                               (@saidaMaterialNfId
                               ,@motivo
                               ,GETDATE())",
                   transaction: Transaction,
                   cancellationToken: cancellationToken,
                   parameters: new
                   {
                       saidaMaterialNfId = saidaMaterialNotaFiscalModel.Id,
                       motivo = Motivo
                   }
               ));


            return Return.Empty;
        }
    }
}
