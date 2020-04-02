using ChatServer.Services.Interfaces;
using ChatServer.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatServer.Models;

namespace ChatServer.Services
{
    public class BlockService : IBlockService
    {
        private readonly IBlockRepository _blockRepository;

        public BlockService(IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository;
        }

        public async Task Block(string SourceId, string Blocked)
        {
            if(!await IsUserBlocked(Blocked, SourceId))
                await _blockRepository.Block(SourceId, Blocked);
        }

        public async Task<List<BlockModel>> GetBlockedList(string SourceId)
        {
            return await _blockRepository.GetBlockedList(SourceId);
        }

        public async Task<bool> IsUserBlocked(string BlockedUser, string BlockedBy)
        {
            return await _blockRepository.IsUserBlocked(BlockedUser, BlockedBy);
        }

        public async Task RemoveBlock(string SourceId, string Blocked)
        {
            await _blockRepository.RemoveBlock(SourceId, Blocked);
        }
    }
}
