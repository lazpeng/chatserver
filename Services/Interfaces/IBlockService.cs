using ChatServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Services.Interfaces
{
    public interface IBlockService
    {
        Task Block(string SourceId, string Blocked);
        Task RemoveBlock(string SourceId, string Blocked);
        Task<List<BlockModel>> GetBlockedList(string SourceId);
        Task<bool> IsUserBlocked(string BlockedUser, string BlockedBy);
    }
}
