using ChatServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatServer.Repositories.Interfaces
{
    public interface IBlockRepository
    {
        Task Block(string SourceId, string Blocked);
        Task RemoveBlock(string SourceId, string Blocked);
        Task<List<BlockModel>> GetBlockedList(string SourceId);
        Task<bool> IsUserBlocked(string BlockedUser, string BlockedBy);
    }
}
