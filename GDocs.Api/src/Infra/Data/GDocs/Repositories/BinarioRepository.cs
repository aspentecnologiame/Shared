using Dapper;
using ICE.Framework.Security.Cryptography;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories
{
    internal class BinarioRepository : Repository, IBinarioRepository
    {
        public BinarioRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<long>> Inserir(byte[] binario, CancellationToken cancellationToken)
        {
            var hash = HashingSHA1ManagedHelper.GenerateHash(Convert.ToBase64String(binario));

            return await _db.Connection.QueryFirstAsync<long>(
                new CommandDefinition(
                    commandText: @"INSERT [dbo].[tb_bin_binario] (bin_val,bin_hash_sha1) VALUES(@binario, @hash);
				        SELECT CAST(SCOPE_IDENTITY() as int)",
                    parameters: new
                    {
                        binario,
                        hash
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );
        }

        public async Task<TryException<byte[]>> ObterPorId(long binarioId, CancellationToken cancellationToken)
         => (await _db.Connection.QueryAsync<byte[]>(
                new CommandDefinition(
                        commandText: @"
                                SELECT 
	                                bin_val 
                                FROM 
	                                tb_bin_binario
                                WHERE
	                                bin_idt = @binarioId",
                        parameters: new
                        {
                            binarioId
                        },
                        transaction: Transaction,
                        cancellationToken: cancellationToken
                    ))).FirstOrDefault();
    }
}
