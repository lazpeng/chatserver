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
        public void EnsureDatabase()
        {
            EnsureSchema();

            var query = @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                                WHERE TABLE_SCHEMA = 'chat' AND TABLE_NAME = 'MESSAGES') 
                            CREATE TABLE chat.MESSAGES (
                                Id BigInt IDENTITY(1,1) PRIMARY KEY,
                                SourceId CHAR(36) FOREIGN KEY REFERENCES chat.USERS (Id) ON DELETE NO ACTION,
                                TargetId CHAR(36) FOREIGN KEY REFERENCES chat.USERS (Id) ON DELETE NO ACTION,
                                DateSent DATETIME NOT NULL,
                                DateSeen DATETIME,
                                Content TEXT NOT NULL,
                                InReplyTo BigInt FOREIGN KEY REFERENCES chat.MESSAGES (Id)
                            )";

            using var conn = GetConnection();
            conn.Execute(query);
        }

        public void SendMessage(SendMessageRequest request)
        {
            EnsureDatabase();

            using var conn = GetConnection();

            var query = @"INSERT INTO chat.MESSAGES (SourceId, TargetId, DateSent, Content, InReplyTo) 
                            VALUES (@SourceId, @TargetId, @DateSent, @Content, @ReplyTo)";

            conn.Execute(query, request);
        }

        public CheckNewResponse CheckNewMessages(CheckNewRequest request)
        {
            EnsureDatabase();

            using var conn = GetConnection();

            var query = "SELECT MAX(Id) FROM chat.MESSAGES WHERE SourceId=@SourceId AND TargetId=@TargetId AND DateSeen IS NOT NULL";

            ulong lastSeenId;
            using(var cmd = GetCommand(query, conn))
            {
                cmd.Parameters.Add(GetParameter("@SourceId", request.SourceId));
                cmd.Parameters.Add(GetParameter("@TargetId", request.TargetId));

                lastSeenId = Convert.ToUInt64(cmd.ExecuteScalar());
            }

            query = "SELECT * FROM chat.MESSAGES WHERE SourceId=@TargetId AND TargetId=@SourceId AND Id > @LastReceivedId";
            var newMessages = conn.Query<MessageModel>(query, request).ToList();

            return new CheckNewResponse { NewMessages = newMessages, TargetLastReadId = lastSeenId };
        }

        public void UpdateSeen(UpdateSeenRequest request)
        {
            EnsureDatabase();

            var conn = GetConnection();

            var query = "UPDATE chat.MESSAGES SET DateSeen = SYSDATETIME() WHERE Id < @LastSeenId AND SourceId=@TargetId AND TargetId=@TargetId";

            conn.Execute(query, request);
        }
    }
}
