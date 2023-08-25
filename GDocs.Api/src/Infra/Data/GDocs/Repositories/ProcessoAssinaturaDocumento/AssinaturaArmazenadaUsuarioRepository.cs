using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.GDocs.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.Repositories.ProcessoAssinaturaDocumento
{
    internal class AssinaturaArmazenadaUsuarioRepository : Repository, IAssinaturaArmazenadaUsuarioRepository
    {
        private const string QUERY_LISTAGEM_PADRAO = @"
                     SELECT 
                        aus_guid_ad AS Guid,
                        aus.bin_idt AS BinarioId,
                        bin.bin_val AS AssinaturaArmazenadaBinario,
                        aus.bin_idt_assinaturadocumento AS BinarioAssinaturaDocumentoId,
                        bin_ass_doc.bin_val AS AssinaturaDocumentoArmazenadaBinario
                     FROM
                        tb_aus_assinatura_usuario aus
                     INNER JOIN tb_bin_binario bin
                        ON aus.bin_idt = bin.bin_idt
                     LEFT JOIN tb_bin_binario bin_ass_doc
                        ON aus.bin_idt_assinaturadocumento = bin_ass_doc.bin_idt
                     WHERE
                        aus_guid_ad {0}";

        public AssinaturaArmazenadaUsuarioRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<AssinaturaArmazenadaUsuarioModel>> ListarPorUsuario(Guid guid, CancellationToken cancellationToken)
        {
            var sql = string.Format(QUERY_LISTAGEM_PADRAO, " = @guid");

            var result = await _db.Connection.QueryAsync<AssinaturaArmazenadaUsuarioModel>(
                new CommandDefinition(
                    commandText: sql,
                    parameters: new
                    {
                        guid
                    },
                    cancellationToken: cancellationToken,
                    transaction: Transaction
                )
            );

            return result?.FirstOrDefault();
        }

        public async Task<TryException<IEnumerable<AssinaturaArmazenadaUsuarioModel>>> ListarPorListagemDeUsuario(List<Guid> listaGuid, CancellationToken cancellationToken)
        {
            var sql = string.Format(QUERY_LISTAGEM_PADRAO, " in @listaGuid");

            var result = await _db.Connection.QueryAsync<AssinaturaArmazenadaUsuarioModel>(
                new CommandDefinition(
                    commandText: sql,
                    parameters: new
                    {
                        listaGuid
                    },
                    cancellationToken: cancellationToken,
                    transaction: Transaction
                )
            );

            return result?.ToCollection();
        }

		public async Task<TryException<Return>> Salvar(long binarioId, Guid usuarioGuid, CancellationToken cancellationToken)
        {
            string sql = $@"                
                MERGE INTO [dbo].[tb_aus_assinatura_usuario] AS Target
                USING 
                (
	                VALUES  (@usuarioGuid)
                ) AS Source 
                ([aus_guid_ad])
							
                ON Target.[aus_guid_ad] = Source.[aus_guid_ad]

                --ativa registros que existem no Target e no Source
                WHEN MATCHED THEN
	                UPDATE SET [bin_idt] = @binarioId ,[aus_dat_atualizacao] = GETDATE()                                                                
                --inseri novos registros que não existem no target e existem no source
                WHEN NOT MATCHED BY TARGET THEN 
	                INSERT 
                        ([aus_guid_ad]
		                ,[bin_idt]
		                )
                    VALUES
                        (@usuarioGuid
		                ,@binarioId);
               ";

            await _db.Connection.QueryAsync<AssinaturaPassoItemModel>(
                new CommandDefinition(
                    commandText: sql,
                    parameters: new
                    {
                        binarioId,
                        usuarioGuid
                    },
                    cancellationToken: cancellationToken,
                    transaction: Transaction
                )
            );

            return Return.Empty;
        }
    }
}
