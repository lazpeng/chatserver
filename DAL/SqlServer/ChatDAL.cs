using ChatServer.Models.Requests;
using ChatServer.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ChatServer.Models;
using ChatServer.DAL.Interfaces;

namespace ChatServer.DAL.SqlServer
{
    public class ChatDAL : BaseDAL, IChatDAL
    {
        public MessageModel SendMessage(SendMessageRequest request)
        {
            using var conn = GetConnection();

            var query = @"INSERT INTO chat.MESSAGES (SourceId, TargetId, DateSent, Content, InReplyTo) 
                            OUTPUT INSERTED.Id, INSERTED.SourceId, INSERTED.Content, INSERTED.DateSent, INSERTED.InReplyTo 
                            VALUES (@SourceId, @TargetId, @DateSent, @Content, @ReplyTo)";

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

            var query = "SELECT ISNULL(MAX(Id), 0) FROM chat.MESSAGES WHERE SourceId=@SourceId AND TargetId=@TargetId AND DateSeen IS NOT NULL";

            long lastSeenId;
            using(var cmd = GetCommand(query, conn))
            {
                cmd.Parameters.Add(GetParameter("@SourceId", request.SourceId));
                cmd.Parameters.Add(GetParameter("@TargetId", request.TargetId));

                lastSeenId = Convert.ToInt64(cmd.ExecuteScalar());
            }

            query = "SELECT * FROM chat.MESSAGES WHERE SourceId=@TargetId AND TargetId=@SourceId AND Id > @LastReceivedId";
            var newMessages = conn.Query<MessageModel>(query, request).ToList();

            return new CheckNewResponse { NewMessages = newMessages, TargetLastReadId = lastSeenId };
        }

        public void UpdateSeen(UpdateSeenRequest request)
        {
            var conn = GetConnection();

            var query = "UPDATE chat.MESSAGES SET DateSeen = SYSDATETIME() WHERE Id <= @LastSeenId AND SourceId=@TargetId AND TargetId=@SourceId";

            conn.Execute(query, request);
        }
    }
}
