using ICE.GDocs.Domain.Core.Transactions;
using System;
using System.Data;

namespace ICE.GDocs.Infra.Data.Core.Transactions
{
    internal class DataTransaction : IDataTransaction
    {
        bool disposed = false;
        bool completed = false;

        protected readonly IDbTransaction _transaction;

        public delegate void CompletedTransactionHandler(object sender, EventArgs e);
        public event CompletedTransactionHandler CompletedTransactionEvent;

        public DataTransaction(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Commit()
        {
            if (completed)
                return;

            _transaction.Commit();

            completed = true;

            CompletedTransactionEvent?.Invoke(this, System.EventArgs.Empty);
        }


        public void Rollback()
        {
            if (completed)
                return;
            _transaction.Dispose();
            completed = true;

            CompletedTransactionEvent?.Invoke(this, System.EventArgs.Empty);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Rollback();
            }

            disposed = true;
        }
    }
}
