using System.Data;

namespace ICE.GDocs.Infra.Data.Core.Database
{
    public interface ICoreDatabase
    {
        IDbConnection Connection { get; }
    }
}
