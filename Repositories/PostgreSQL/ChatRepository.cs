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

        public CheckNewResponse CheckNewMessages(CheckNewRequest request)
        {
            using var conn = GetConnection();

            var query = "SELECT * FROM chat.MESSAGES WHERE SourceId=@TargetId AND Id > @LastReceivedId";
            var newMessages = conn.Query<MessageModel>(query, request).ToList();

            return new CheckNewResponse { NewMessages = newMessages };
        }

        public void UpdateSeenMessage(UpdateSeenRequest request)
        {
            var conn = GetConnection();

            var query = "UPDATE chat.MESSAGES SET DateSeen = CURRENT_TIMESTAMP() WHERE Id <= @LastSeenId AND SourceId=@TargetId AND TargetId=@SourceId";

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
    }
}
