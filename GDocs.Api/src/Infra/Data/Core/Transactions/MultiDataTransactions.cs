using ICE.GDocs.Domain.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICE.GDocs.Infra.Data.Core.Transactions
{
    internal class MultiDataTransactions : IDataTransaction
    {
        bool disposed = false;

        private readonly List<IDataTransaction> _transactions;

        public delegate void CompletedTransactionHandler(object sender, EventArgs e);
        public event CompletedTransactionHandler CompletedTransactionEvent;

        public MultiDataTransactions(List<IDataTransaction> transactions)
        {
            _transactions = transactions;
        }

        public void Commit()
        {
            if (!_transactions.Any())
                return;

            _transactions.ForEach(transaction => transaction?.Commit());
            _transactions.Clear();

            CompletedTransactionEvent?.Invoke(this, EventArgs.Empty);
        }


        public void Rollback()
        {
            if (!_transactions.Any())
                return;

            _transactions.ForEach(transaction => transaction?.Dispose());
            _transactions.Clear();

            CompletedTransactionEvent?.Invoke(this, EventArgs.Empty);
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
