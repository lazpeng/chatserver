using ChatServer.Models;
using ChatServer.Models.Requests;
using ChatServer.Models.Responses;
using ChatServer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace ChatServer.Repositories.PostgreSQL
{
    public class ChatRepository : BaseRepository, IChatRepository
    {
        public ChatRepository(string ConnectionString) : base(ConnectionString) { }

        public MessageModel SendMessage(SendMessageRequest request)
        {
            using var conn = GetConnection();

            var query = @"INSERT INTO chat.MESSAGES (SourceId, TargetId, DateSent, Content, InReplyTo) 
                            VALUES (@SourceId, @TargetId, @DateSent, @Content, @ReplyTo)
                            RETURNING Id, SourceId, Content, DateSent, InReplyTo";

            var results = conn.Query<MessageModel>(query, request);

            if (results.Any())
            {
                return results.First();
            }
            else return null;
        }

        public void UpdateSeenMessage(UpdateSeenRequest request)
        {
            var conn = GetConnection();

            var query = "UPDATE chat.MESSAGES SET DateSeen = CURRENT_TIMESTAMP WHERE Id <= @LastSeenId AND SourceId=@TargetId AND TargetId=@SourceId";

            conn.Execute(query, request);
        }

        public LastSeenResponse GetLastSeenMessage(GetLastSeenRequest request)
        {
            using var conn = GetConnection();

            var query = "SELECT Max(Id) as LastSeenId FROM chat.MESSAGES WHERE SourceId=@SourceId AND TargetId=@TargetId AND DateSeen IS NOT NULL";

            var results = conn.Query<LastSeenResponse>(query, request);
            if (results.Any())
            {
                return results.First();
            }
            else return new LastSeenResponse { LastSeenId = -1 };
        }

        public List<MessageModel> GetSentMessages(string UserId, long OffsetId)
        {
            using var conn = GetConnection();

            var query = "SELECT * FROM chat.MESSAGES WHERE TargetId=@UserId AND Id > @OffsetId";

            return conn.Query<MessageModel>(query, new { UserId, OffsetId }).ToList();
        }

        public List<MessageModel> GetEditedMessages(string UserId, long OffsetId)
        {
            using var conn = GetConnection();

            var query = "SELECT m.Id as Id, m.SourceId as SourceId, e.NewContent as Content, e.DateEdited as DateSent, m.InReplyTo as InReplyTo" +
                        " FROM chat.EDITED e INNER JOIN chat.MESSAGES m ON e.MessageId = m.Id WHERE m.TargetId = @UserId and e.Id > @OffsetId";

            return conn.Query<MessageModel>(query, new { UserId, OffsetId }).ToList();
        }

        public List<long> GetDeletedMessages(string UserId, long OffsetId)
        {
            using var conn = GetConnection();

            var query = "SELECT m.Id FROM chat.DELETED d INNER JOIN chat.MESSAGES m ON d.MessageId = m.Id WHERE m.TargetId = @UserId AND d.Id > @OffsetId";

            return conn.Query<long>(query, new { UserId, OffsetId }).ToList();
        }

        public void EditMessage(EditMessageRequest request)
        {
            using var conn = GetConnection();

            var query = "INSERT INTO chat.EDITED (MessageId, NewContent, DateEdited) VALUES (@MessageId, @NewContent, @DateEdited)";

            conn.Execute(query, request);
        }

        public bool IsMessageDeleted(long Id)
        {
            using var conn = GetConnection();

            var query = "SELECT COUNT(*) FROM chat.DELETED WHERE MessageId = @Id";

            return conn.QuerySingle<long>(query, new { Id }) != 0;
        }

        public void DeleteMessage(DeleteMessageRequest request)
        {
            using var conn = GetConnection();

            var query = "INSERT INTO chat.DELETED (MessageId, DateDeleted) VALUES (@MessageId, @DateDeleted)";

            conn.Execute(query, request);
        }

        public string GetSentUserId(long MessageId)
        {
            var query = "SELECT SourceId FROM chat.MESSAGES WHERE Id=@MessageId";

            using var conn = GetConnection();

            return conn.QuerySingleOrDefault<string>(query, new { MessageId });
        }
    }
}
