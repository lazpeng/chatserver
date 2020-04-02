using ChatServer.Models;
using ChatServer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace ChatServer.Repositories.PostgreSQL
{
    public class FriendRepository : BaseRepository, IFriendRepository
    {
        public FriendRepository(IConnectionStringProvider provider) : base(provider) { }

        public async Task AnswerFriendRequest(long Id, bool Accepted)
        {
            using var connection = await GetConnection();

            await connection.ExecuteAsync("UPDATE chat.REQUESTS SET AnswerDate = CURRENT_TIMESTAMP," +
                " Accepted = @Accepted, Active = FALSE WHERE Id = @Id",
                new { Id, Accepted });

            if(Accepted)
            {
                await connection.ExecuteAsync(
                    @"WITH Request AS (
                        SELECT * FROM chat.REQUESTS WHERE Id = @Id
                    )
                    INSERT INTO chat.FRIENDS (A, B, SentDate, AcceptDate) VALUES ((SELECT SourceId FROM Request),
                        (SELECT TargetId FROM Request), (SELECT SentDate FROM Request), CURRENT_TIMESTAMP)
                    "
                , new { Id });
            }
        }

        public async Task<bool> AreUsersFriends(string A, string B)
        {
            using var connection = await GetConnection();

            var query = "SELECT COUNT(*) FROM chat.FRIENDS WHERE (A = @A AND B = @B) OR (A = @B AND B = @A)";

            return Convert.ToInt32(await connection.QuerySingleAsync<long>(query, new { A, B })) > 0;
        }

        public async Task DeleteFriendRequest(long Id)
        {
            using var connection = await GetConnection();

            var query = "UPDATE chat.REQUESTS SET Active = FALSE WHERE Id=@Id";

            await connection.ExecuteAsync(query, new { Id });
        }

        public async Task<FriendRequestModel> GetRequest(long Id)
        {
            using var connection = await GetConnection();

            var query = "SELECT * FROM chat.REQUESTS WHERE Id = @Id";

            return await connection.QuerySingleAsync<FriendRequestModel>(query, new { Id });
        }

        public async Task<FriendRequestModel> FindRequest(string SourceId, string TargetId)
        {
            using var connection = await GetConnection();

            var query = "SELECT * FROM chat.REQUESTS WHERE SourceId = @SourceId And TargetId = @TargetId AND Active = TRUE ORDER BY DateSent DESC";

            return await connection.QuerySingleAsync<FriendRequestModel>(query, new { SourceId, TargetId });
        }

        public async Task<List<FriendModel>> GetFriendList(string User)
        {
            using var connection = await GetConnection();

            var query = "SELECT * FROM chat.FRIENDS WHERE A = @User OR B = @User";

            return (await connection.QueryAsync<FriendModel>(query, new { User })).ToList();
        }

        public async Task<List<FriendRequestModel>> GetFriendRequests(string User)
        {
            using var connection = await GetConnection();

            var query = "SELECT * FROM chat.REQUESTS WHERE SourceId = @User OR TargetId = @User";

            return (await connection.QueryAsync<FriendRequestModel>(query, new { User })).ToList();
        }

        public async Task<bool> HasPendingRequest(string SourceId, string TargetId)
        {
            using var connection = await GetConnection();

            var query = "SELECT COUNT(*) FROM chat.REQUESTS WHERE SourceId = @SourceId AND TargetId = @TargetId AND Active = TRUE";

            return Convert.ToInt32(await connection.QuerySingleAsync<long>(query, new { SourceId, TargetId })) > 0;
        }

        public async Task<FriendRequestModel> SendFriendRequest(string SourceId, string TargetId)
        {
            using var connection = await GetConnection();

            var query = "INSERT INTO chat.REQUESTS (SourceId, TargetId, SentDate) VALUES (@SourceId, @TargetId, CURRENT_TIMESTAMP)";

            return await connection.QuerySingleAsync<FriendRequestModel>(query, new { SourceId, TargetId });
        }

        public async Task RemoveFriend(string A, string B)
        {
            using var connection = await GetConnection();

            var query = "DELETE FROM chat.FRIENDS WHERE (A = @A AND B = @B) OR (A = @B AND B = @A)";

            await connection.ExecuteAsync(query, new { A, B });
        }
    }
}
