using ICE.GDocs.Domain.Core.Transactions;
using System;

namespace ICE.GDocs.Domain.Core.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        IDataTransaction BeginTransaction();

        IDataTransaction DataTransaction { get; }

        void Commit();

        void Rollback();
    }
}
