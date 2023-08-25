using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Domain.Core.Transactions;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using System.Data;

namespace ICE.GDocs.Infra.Data.Core.Repositories
{
    internal class Repository : IRepository
    {
        protected readonly IGDocsDatabase _db;
        protected readonly IUnitOfWork _unitOfWork;

        protected Repository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        )
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }

        public IDbTransaction Transaction
        {
            get
            {
                if (_unitOfWork.DataTransaction is IDapperDataTransaction)
                {
                    var dapperDataTransaction = _unitOfWork.DataTransaction as IDapperDataTransaction;

                    return dapperDataTransaction.Transaction;
                }

                return null;
            }
        }
    }
}
