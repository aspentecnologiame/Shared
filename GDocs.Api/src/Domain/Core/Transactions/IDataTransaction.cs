using System;

namespace ICE.GDocs.Domain.Core.Transactions
{
    public interface IDataTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
