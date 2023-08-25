using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.Repositories.ProcessoAssinaturaDocumento
{
    internal class ArquivoRepository : Repository, IArquivoRepository
    {
        public ArquivoRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<IEnumerable<AssinaturaArquivoModel>>> Salvar(int processoAssinaturaDocumentoId, IEnumerable<AssinaturaArquivoModel> arquivos, CancellationToken cancellationToken ,bool edicao = false)
        {
            string sql = $@"                
                MERGE INTO [dbo].[tb_pada_processo_assinatura_documento_arquivo] AS Target
                USING 
                (
	                VALUES { string.Join(",", arquivos.ConvertAll(item => $"({processoAssinaturaDocumentoId},{item.Ordem},{item.BinarioId})"))}
                ) AS Source 
                ([pad_idt]
                ,[pada_ordem]
                ,[bin_idt])
							
                ON Target.[pad_idt] = Source.[pad_idt] AND Target.bin_idt = Source.bin_idt

                --ativa registros que existem no Target e no Source
                WHEN MATCHED AND [pada_flg_ativo] = 0 THEN
	                UPDATE SET [pada_flg_ativo] = 1 ,[pada_dat_atualizacao] = GETDATE()
                --inativa registros que somente existem no target e não existem no source
                WHEN NOT MATCHED BY SOURCE AND [pad_idt] = @processoAssinaturaDocumentoId AND [pada_flg_ativo] = 1 THEN
	                UPDATE SET [pada_flg_ativo] = 0 , [pada_dat_atualizacao] = getdate() 		                                
                --inseri novos registros que não existem no target e existem no source
                WHEN NOT MATCHED BY TARGET THEN 
	                INSERT 
                        ([pad_idt]
		                ,[pada_ordem]
		                ,[bin_idt])
                    VALUES
                        ([pad_idt],
                         {(edicao? "0" : "(SELECT ISNULL(MAX(pada_ordem)+1,0) FROM [dbo].[tb_pada_processo_assinatura_documento_arquivo] WHERE pad_idt = @processoAssinaturaDocumentoId AND [pada_flg_final] = 0)")}
		                ,[bin_idt])

                OUTPUT
	                INSERTED.[pada_idt] AS Id,  INSERTED.[bin_idt] AS BinarioId;";

            var ids = await _db.Connection.QueryAsync<AssinaturaArquivoModel>(
                new CommandDefinition(
                    commandText: sql,
                    parameters: new
                    {
                        processoAssinaturaDocumentoId
                    },
                    cancellationToken: cancellationToken,
                    transaction: Transaction
                )
            );

            if (ids.Any())
                arquivos = arquivos.ForEach(item =>
                {
                    item.Id = ids.FirstOrDefault(id => id.BinarioId == item.BinarioId).Id;
                });

            return arquivos?.ToCollection();
        }

        public async Task<TryException<int>> Inativar(long id, CancellationToken cancellationToken)
            => await _db.Connection.ExecuteAsync(
               new CommandDefinition(
                   commandText: @"UPDATE tb_pada_processo_assinatura_documento_arquivo
                                  SET pada_flg_ativo = 0, pada_dat_atualizacao = GETDATE()
                                  WHERE pada_idt = @id",
                   parameters:  new { id },
                   cancellationToken: cancellationToken,
                   transaction: Transaction
               ));        

        public async Task<TryException<IEnumerable<AssinaturaArquivoModel>>> ListarArquivosUploadPorPadId(int processoAssinaturaDocumentoId, CancellationToken cancellationToken, bool listarTodos = false)
        {
            var result = await _db.Connection.QueryAsync<AssinaturaArquivoModel>(
               new CommandDefinition(
                   commandText: $@"
                                SELECT pada_idt AS Id    
                                        ,pada_ordem AS Ordem
                                        ,bin.bin_idt AS BinarioId
                                        ,bin.bin_val AS ArquivoBinario
                                        ,pad.pad_idt AS ProcessoAssinaturaDocumentoId
                                        ,pad.sad_idt AS ProcessoAssinaturaDocumentoStatusId
                                        ,pad.pad_guid_ad AS AutorId
                                        ,pada.pada_flg_final AS ArquivoFinal
                                        ,pada.pada_flg_ativo AS Status
                                        ,pada.pada_dat_atualizacao AS Atualizacao
                                        ,pad.padc_idt AS Categoria
                                FROM 
                                    [dbo].[tb_pada_processo_assinatura_documento_arquivo] pada
                                INNER JOIN 
                                    tb_bin_binario bin ON bin.bin_idt = pada.bin_idt
                                INNER JOIN 
                                    tb_pad_processo_assinatura_documento pad ON pad.pad_idt = pada.pad_idt
                                WHERE 
                                    (@listarTodos = 1 OR pada_flg_ativo = 1) AND
                                    pad_flg_ativo = 1 AND
                                    pada.pad_idt=@processoAssinaturaDocumentoId",
                   parameters: new
                   {
                       processoAssinaturaDocumentoId,
                       listarTodos
                   },
                   cancellationToken: cancellationToken
               )
           );

            return result?.ToCollection();
        }

        public async Task<TryException<IEnumerable<AssinaturaArquivoModel>>> ListarArquivoExpurgoPorPad(int processoAssinaturaDocumentoId, CancellationToken cancellationToken, bool listarTodos = false)
        {
            var result = await _db.Connection.QueryAsync<AssinaturaArquivoModel>(
               new CommandDefinition(
                   commandText: $@"
                                SELECT pada_idt AS Id    
                                        ,pada_ordem AS Ordem
                                        ,bin.bin_idt AS BinarioId
                                        ,bin.bin_val AS ArquivoBinario
                                        ,pad.pad_idt AS ProcessoAssinaturaDocumentoId
                                        ,pad.sad_idt AS ProcessoAssinaturaDocumentoStatusId
                                        ,pad.pad_guid_ad AS AutorId
                                        ,pada.pada_flg_final AS ArquivoFinal
                                        ,pada.pada_flg_ativo AS Status
                                        ,pada.pada_dat_atualizacao AS Atualizacao
                                        ,pad.padc_idt AS Categoria
                                FROM 
                                    [dbo].[tb_pada_processo_assinatura_documento_arquivo] pada
                                LEFT JOIN 
                                    tb_bin_binario bin ON bin.bin_idt = pada.bin_idt
                                INNER JOIN 
                                    tb_pad_processo_assinatura_documento pad ON pad.pad_idt = pada.pad_idt
                                WHERE 
                                    (@listarTodos = 1 OR pada_flg_ativo = 1) AND
                                    pad_flg_ativo = 1 AND
                                    pada.pad_idt=@processoAssinaturaDocumentoId",
                   parameters: new
                   {
                       processoAssinaturaDocumentoId,
                       listarTodos
                   },
                   cancellationToken: cancellationToken
               )
           );

            return result?.ToCollection();
        }
    }
}
