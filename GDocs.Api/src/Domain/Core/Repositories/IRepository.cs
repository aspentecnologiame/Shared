using System.Data;

namespace ICE.GDocs.Domain.Core.Repositories
{
    public interface IRepository
    {
        IDbTransaction Transaction { get; }
    }
}
