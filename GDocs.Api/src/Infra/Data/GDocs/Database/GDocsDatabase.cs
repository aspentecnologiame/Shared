using ICE.GDocs.Domain.Database;
using ICE.GDocs.Infra.Data.Core.Database;

namespace ICE.GDocs.Infra.Data.Database
{
    internal class GDocsDatabase : CoreDatabase, IGDocsDatabase
    {
        public GDocsDatabase(string strConnection)
            : base(strConnection)
        {
        }
    }
}
