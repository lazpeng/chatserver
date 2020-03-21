using System.Data;
using System.Threading.Tasks;

namespace ChatServer.Repositories.Interfaces
{
    public interface IBaseRepository
    {
        Task<IDbConnection> GetConnection();
        Task PerformUpgrade();
        Task<long> GetDatabaseVersion();
        long LatestDatabaseVersion { get; }
    }
}
