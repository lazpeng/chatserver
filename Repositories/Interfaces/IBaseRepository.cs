using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Repositories.Interfaces
{
    public interface IBaseRepository
    {
        IDbConnection GetConnection();
        IDbCommand GetCommand(string Query, IDbConnection Connection);
        IDbDataParameter GetParameter(string Name, object Value);
        void PerformUpgrade();
        long GetDatabaseVersion();
        long LatestDatabaseVersion { get; }
    }
}
