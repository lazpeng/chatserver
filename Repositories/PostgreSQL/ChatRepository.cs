using ChatServer.Models;
using ChatServer.Models.Requests;
using ChatServer.Models.Responses;
using ChatServer.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace ChatServer.Repositories.PostgreSQL
{
    public class ChatRepository : BaseRepository, IChatRepository
    {
        public ChatRepository(IConnectionStringProvider provider) : base(provider) { }

        public async Task<MessageModel> SendMessage(SendMessageRequest request)
        {
            using var conn = await GetConnection();

            var query = @"INSERT INTO chat.MESSAGES (SourceId, TargetId, DateSent, Content, InReplyTo) 
                            VALUES (@SourceId, @TargetId, @DateSent, @Content, @ReplyTo)
                            RETURNING Id, SourceId, Content, DateSent, InReplyTo";

            var results = await conn.QueryAsync<MessageModel>(query, request);

            if (results.Any())
            {
                return results.First();
            }
            else return null;
        }

        public async Task UpdateSeenMessage(UpdateSeenRequest request)
        {
            using var conn = await GetConnection();

            await conn.ExecuteAsync("UPDATE chat.MESSAGES SET DateSeen = CURRENT_TIMESTAMP WHERE Id <= @LastSeenId AND SourceId=@TargetId AND TargetId=@SourceId", request);
        }

        public async Task<LastSeenResponse> GetLastSeenMessage(GetLastSeenRequest request)
        {
            using var conn = await GetConnection();

            var result = await conn.QuerySingleAsync<long>("SELECT COALESCE(Max(Id), -1) as LastSeenId FROM chat.MESSAGES WHERE SourceId=@SourceId AND TargetId=@TargetId AND DateSeen IS NOT NULL", request);
            return new LastSeenResponse { LastSeenId = result };
        }

        public async Task<List<MessageModel>> GetSentMessages(string UserId, long OffsetId)
        {
            using var conn = await GetConnection();

            return (await conn.QueryAsync<MessageModel>("SELECT * FROM chat.MESSAGES WHERE TargetId=@UserId AND Id > @OffsetId", new { UserId, OffsetId })).ToList();
        }

        public async Task<List<MessageModel>> GetEditedMessages(string UserId, long OffsetId)
        {
            using var conn = await GetConnection();

            var query = "SELECT m.Id as Id, m.SourceId as SourceId, e.NewContent as Content, e.DateEdited as DateSent, m.InReplyTo as InReplyTo" +
                        " FROM chat.EDITED e INNER JOIN chat.MESSAGES m ON e.MessageId = m.Id WHERE m.TargetId = @UserId and e.Id > @OffsetId";

            return (await conn.QueryAsync<MessageModel>(query, new { UserId, OffsetId })).ToList();
        }

        public async Task<List<long>> GetDeletedMessages(string UserId, long OffsetId)
        {
            using var conn = await GetConnection();

            var query = "SELECT m.Id FROM chat.DELETED d INNER JOIN chat.MESSAGES m ON d.MessageId = m.Id WHERE m.TargetId = @UserId AND d.Id > @OffsetId";

            return (await conn.QueryAsync<long>(query, new { UserId, OffsetId })).ToList();
        }

        public async Task EditMessage(EditMessageRequest request)
        {
            using var conn = await GetConnection();

            var query = "INSERT INTO chat.EDITED (MessageId, NewContent, DateEdited) VALUES (@MessageId, @NewContent, @DateEdited)";

            await conn.ExecuteAsync(query, request);
        }

        public async Task<bool> IsMessageDeleted(long Id)
        {
            using var conn = await GetConnection();

            var query = "SELECT COUNT(*) FROM chat.DELETED WHERE MessageId = @Id";

            return await conn.QuerySingleAsync<long>(query, new { Id }) != 0;
        }

        public async Task DeleteMessage(DeleteMessageRequest request)
        {
            using var conn = await GetConnection();

            var query = "INSERT INTO chat.DELETED (MessageId, DateDeleted) VALUES (@MessageId, @DateDeleted)";

            await conn.ExecuteAsync(query, request);
        }

        public async Task<string> GetSentUserId(long MessageId)
        {
            var query = "SELECT SourceId FROM chat.MESSAGES WHERE Id=@MessageId";

            using var conn = await GetConnection();

            return await conn.QuerySingleOrDefaultAsync<string>(query, new { MessageId });
        }
    }
}
