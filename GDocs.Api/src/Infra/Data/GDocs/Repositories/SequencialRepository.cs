using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Data;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.Repositories
{
    internal class SequencialRepository : Repository, ISequencialRepository
    {
        public SequencialRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<int>> ObterProximo(string chave)
        {
            var parametros = new DynamicParameters();
            parametros.Add("nome", chave, DbType.String);
            parametros.Add("valor", dbType: DbType.Int32, direction: ParameterDirection.Output);
            

            await _db.Connection.ExecuteAsync(
                "[dbo].[sp_get_nextval]",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return parametros.Get<int>("@valor");
        }
    }
}
