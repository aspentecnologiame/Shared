using ICE.GDocs.Domain.Core.Transactions;
using System.Data;

namespace ICE.GDocs.Infra.Data.Core.Transactions
{

    internal class DapperDataTransaction : DataTransaction, IDapperDataTransaction
    {
        public DapperDataTransaction(IDbTransaction transaction)
            : base(transaction)
        {

        }

        public IDbTransaction Transaction => _transaction;
    }
}
