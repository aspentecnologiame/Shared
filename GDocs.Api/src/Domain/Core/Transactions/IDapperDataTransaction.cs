using System.Data;

namespace ICE.GDocs.Domain.Core.Transactions
{
    public interface IDapperDataTransaction : IDataTransaction
    {
        IDbTransaction Transaction { get; }
    }
}
