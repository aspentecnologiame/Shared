using ICE.GDocs.Domain.Core.Uow;

namespace ICE.GDocs.Domain.Core.Services
{
    internal class DomainService : IDomainService
    {
        protected readonly IUnitOfWork _unitOfWork;

        protected DomainService(
            IUnitOfWork unitOfWork
        )
        {
            _unitOfWork = unitOfWork;
        }
    }
}
