using System.Data;

namespace ICE.GDocs.Infra.Data.Core.Database
{
    internal class CoreDatabase : ICoreDatabase
    {
        public IDbConnection Connection { get; private set; }

        public CoreDatabase(string strConnection)
        {
            Connection = new System.Data.SqlClient.SqlConnection(strConnection);
        }
    }
}