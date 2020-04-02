using ChatServer.Models;
using ChatServer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace ChatServer.Repositories.PostgreSQL
{
    public class BlockRepository : BaseRepository, IBlockRepository
    {
        public BlockRepository(IConnectionStringProvider provider) : base(provider) { }

        public async Task Block(string SourceId, string Blocked)
        {
            using var conn = await GetConnection();

            var query = "INSERT INTO chat.BLOCKLIST (UserId, BlockedId, BlockDate) VALUES (@SourceId, @Blocked, CURRENT_TIMESTAMP)";

            await conn.ExecuteAsync(query, new { SourceId, Blocked });
        }

        public async Task<List<BlockModel>> GetBlockedList(string SourceId)
        {
            using var conn = await GetConnection();

            var query = "SELECT * FROM chat.BLOCKLIST WHERE UserId = @SourceId";

            return (await conn.QueryAsync<BlockModel>(query, new { SourceId })).ToList();
        }

        public async Task<bool> IsUserBlocked(string BlockedUser, string BlockedBy)
        {
            using var conn = await GetConnection();

            var query = "SELECT COUNT(*) FROM chat.BLOCKLIST WHERE UserId = @BlockedBy AND BlockedId = @BlockedUser";

            return Convert.ToInt32(await conn.QuerySingleAsync<long>(query, new { BlockedUser, BlockedBy })) > 0;
        }

        public async Task RemoveBlock(string SourceId, string Blocked)
        {
            using var conn = await GetConnection();

            var query = "DELETE FROM chat.BLOCKLIST WHERE UserId = @SourceId AND BlockedId = @Blocked";

            await conn.ExecuteAsync(query, new { SourceId, Blocked });
        }
    }
}
