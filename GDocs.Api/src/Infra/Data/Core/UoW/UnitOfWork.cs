using ICE.GDocs.Domain.Core.Transactions;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Infra.Data.Core.Transactions;
using System;

namespace ICE.GDocs.Infra.Data.Core.UoW
{
    internal class UnitOfWork : IUnitOfWork
    {
        bool disposed = false;

        private IDataTransaction _activeTransaction;

        private readonly IGDocsDatabase _db;

        public UnitOfWork(
            IGDocsDatabase db
        )
        {
            _db = db;

            if (_db.Connection.State == System.Data.ConnectionState.Closed)
                _db.Connection.Open();
        }

        public IDataTransaction DataTransaction => _activeTransaction;

        public IDataTransaction BeginTransaction()
            => _activeTransaction = _activeTransaction ?? CreateTransacion();

        public void Commit()
            => _activeTransaction?.Commit();

        public void Rollback()
            => _activeTransaction?.Dispose();

        private IDataTransaction CreateTransacion()
         => new DapperDataTransaction(_db.Connection.BeginTransaction());


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

            if (_db.Connection.State != System.Data.ConnectionState.Closed)
                _db.Connection.Close();

            disposed = true;
        }
    }
}
