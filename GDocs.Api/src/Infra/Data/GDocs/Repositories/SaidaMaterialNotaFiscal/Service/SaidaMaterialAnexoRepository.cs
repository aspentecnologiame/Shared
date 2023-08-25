using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories.SaidaMaterialNotaFiscal.Service
{
    internal class SaidaMaterialAnexoRepository : Repository, ISaidaMaterialAnexoRepository
    {
        public SaidaMaterialAnexoRepository(IGDocsDatabase db,IUnitOfWork unitOfWork) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<IEnumerable<SaidaMaterialArquivoModel>>> ListarAnexo(int saidaMaterialId, CancellationToken cancellationToken) => (
            await _db.Connection.QueryAsync<SaidaMaterialArquivoModel>(
                new CommandDefinition(
                    commandText: $@"
                                SELECT  
                                    smdnf.smdnf_idt [Id],
                                    smdnf.tdnf_idt [TipoNatureza],
                                    tdnf.tdnf_des [NomeSalvo],
                                    tdnf.tdnf_des [NomeOriginal],
                                    bin.bin_val [ArquivoBinario]
                                FROM   
                                    tb_smdnf_saida_material_documento_nota_fiscal smdnf
                                    LEFT JOIN tb_bin_binario bin on bin.bin_idt = smdnf.bin_idt
                                    INNER JOIN tb_tdnf_tipo_documento_nota_fiscal tdnf on tdnf.tdnf_idt = smdnf.tdnf_idt
                                WHERE 
                                    smdnf.smnf_idt = @id
                                    AND smdnf.smdnf_flg_ativo = 1
	                            ORDER BY 
                                smdnf.tdnf_idt
                                DESC",
                    cancellationToken: cancellationToken,
                    parameters: new {id = saidaMaterialId}
                ))).ToCollection();

        public async Task<TryException<SaidaMaterialArquivoModel>> ObterAnexoPorIdEhTipo(int saidaMaterialId, TipoDocAnexo tipoDocAnexo, CancellationToken cancellationToken) => (
            await _db.Connection.QueryAsync<SaidaMaterialArquivoModel>(
                new CommandDefinition(
                    commandText: $@"
                                   SELECT  
                                    smdnf.smdnf_idt [Id],
                                    smdnf.smnf_idt [SaidaMaterialNfId],
                                    smdnf.smnf_numero_documento [Numero],
                                    smdnf.bin_idt [BinarioId],
                                    smdnf.smnf_motivo [Motivo],
                                    smdnf.smdnf_flg_ativo [Ativo],
                                    smdnf.smdnf_dat_inclusao [Criacao],
                                    smdnf.smdnf_dat_alteracao [Atualizacao],
                                    tdnf.tdnf_des [NomeOriginal],
                                    tdnf.tdnf_idt [TipoNatureza]
                                FROM   
                                    tb_smdnf_saida_material_documento_nota_fiscal smdnf
                                    INNER JOIN tb_tdnf_tipo_documento_nota_fiscal tdnf on tdnf.tdnf_idt = smdnf.tdnf_idt
                                WHERE 
                                    smdnf.smnf_idt = @Id
                                    AND smdnf.tdnf_idt = @Tipo
                                    AND smdnf.smdnf_flg_ativo = 1
	                            ORDER BY 
                                smdnf.tdnf_idt
                                DESC",
                    cancellationToken: cancellationToken,
                    transaction: Transaction,
                    parameters: new {
                        Id = saidaMaterialId,
                        Tipo = tipoDocAnexo.ToInt32()
                    }
                ))).FirstOrDefault();

        public async Task<TryException<Return>> InativaAnexo(SaidaMaterialArquivoModel saidaMaterialArquivoModel,  CancellationToken cancellationToken)
        {
            await _db.Connection.ExecuteAsync(
                new CommandDefinition(
                    commandText: @"
                        UPDATE
                            tb_smdnf_saida_material_documento_nota_fiscal 
                        SET 
                            smdnf_flg_ativo = 0,
                            smnf_motivo = @Motivo,
                            smdnf_guid_ad_autor = @Autor
                        WHERE
                        smdnf_idt = @Id",
                    transaction: Transaction,
                    cancellationToken: cancellationToken,
                    parameters: new
                    {
                        Id = saidaMaterialArquivoModel.Id,
                        Motivo = saidaMaterialArquivoModel.Motivo,
                        Autor = saidaMaterialArquivoModel.GuidAutor

                    }
                )
            );

            return Return.Empty;
        }

        public async Task<TryException<SaidaMaterialArquivoModel>> Salvar(SaidaMaterialArquivoModel saidaMaterialArquivoModel, CancellationToken cancellationToken)
        {
            string sql = $@"MERGE INTO [dbo].[tb_smdnf_saida_material_documento_nota_fiscal] AS Target
				USING
				(
			      VALUES
                   (@Id, @SaidaMaterialNfId ,@TipoNatureza, @Numero,  @BinarioId ,@Ativo, @GuidAutor ,GETDATE(), GETDATE())
				) AS Source 
				 ([smdnf_idt],[smnf_idt], [tdnf_idt], [smnf_numero_documento], [bin_idt], [smdnf_flg_ativo], [smdnf_guid_ad_autor] ,[smdnf_dat_inclusao], [smdnf_dat_alteracao])
							
                ON Target.[smdnf_idt] = Source.[smdnf_idt] AND Target.bin_idt = Source.bin_idt

			WHEN MATCHED THEN
					UPDATE SET 
                             [smnf_idt] = Source.[smnf_idt]
                            ,[tdnf_idt] = Source.[tdnf_idt]
                            ,[smnf_numero_documento] = Source.[smnf_numero_documento]            
                            ,[bin_idt] = Source.[bin_idt]
                            ,[smdnf_flg_ativo] = Source.[smdnf_flg_ativo]
                            ,[smdnf_guid_ad_autor] = Source.[smdnf_guid_ad_autor]
                            ,[smdnf_dat_alteracao] = GETDATE()
                
				--inseri novos registros que não existem no target e existem no source
                WHEN NOT MATCHED BY TARGET THEN 
	                INSERT 
                        ([smnf_idt],
						[tdnf_idt],
						[smnf_numero_documento],
						[bin_idt],
						[smdnf_flg_ativo],
                        [smdnf_guid_ad_autor],
						[smdnf_dat_inclusao],
						[smdnf_dat_alteracao])
                    VALUES
                        ([smnf_idt],
						[tdnf_idt],
						[smnf_numero_documento],
						[bin_idt],
						[smdnf_flg_ativo],
                        [smdnf_guid_ad_autor],
						GETDATE(),
						GETDATE())

                OUTPUT
	                INSERTED.[smdnf_idt] AS Id,  INSERTED.[bin_idt] AS BinarioId;";

            var id = await _db.Connection.QuerySingleAsync<int>(new CommandDefinition(
            commandText: sql,
            parameters: saidaMaterialArquivoModel,
            transaction: Transaction,
            cancellationToken: cancellationToken));


            saidaMaterialArquivoModel.DefinirId(id);

            return saidaMaterialArquivoModel;
        }

        public async Task<TryException<int>> CheckarDuplicidade(int smnfIdt, int smnfNumeroDocumento, CancellationToken cancellationToken)
        {
            string sql = $@"SELECT CAST(COUNT(smdnf_idt) AS INT) FROM tb_smdnf_saida_material_documento_nota_fiscal WHERE smnf_idt = @SmnfIdt AND smnf_numero_documento = @SmnfNumeroDocumento";
            
            return await _db.Connection.QueryFirstAsync<int>(new CommandDefinition(
                commandText: sql,
                parameters: new { SmnfIdt = smnfIdt, smnfNumeroDocumento = smnfNumeroDocumento },
                transaction: Transaction,
                cancellationToken: cancellationToken));
        }
    }
}



