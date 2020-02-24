using System;
using Dapper;
using System.Linq;
using ChatServer.Models;
using ChatServer.Exceptions;

namespace ChatServer.DAL
{
    public abstract class UserDAL : BaseDAL
    {
        private static void EnsureDatabase()
        {
            var usersTable = @"CREATE TABLE IF NOT EXISTS USERS (
                            Id CHAR(36) PRIMARY KEY,
                            UserName VARCHAR(128) NOT NULL,
                            FullName VARCHAR(256) NOT NULL,
                            Email VARCHAR(256),
                            Bio TEXT,
                            AccountCreated TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            LastLogin TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            LastSeen TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            ProfilePicUrl TEXT,
                            FindInSearch BOOL NOT NULL DEFAULT TRUE,
                            OpenChat BOOL NOT NULL DEFAULT TRUE,
                            CONSTRAINT USER_UNIQUE UNIQUE (UserName),
                            CONSTRAINT EMAIL_UNIQUE UNIQUE (Email)
                            )";
            
            var friendsTable = @"CREATE TABLE IF NOT EXISTS FRIENDS (
                                A CHAR(36) FOREIGN KEY REFERENCES USERS (Id),
                                B CHAR(36) FOREIGN KEY REFERENCES USERS (Id),
                                SentDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                SettleDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                Accepted BOOL NOT NULL DEFAULT FALSE,
                                CONSTRAINT FRIENDS_PK PRIMARY KEY (A, B)
                            )";

            
            var blockTable = @"CREATE TABLE IF NOT EXISTS BLACKLIST (
                                User CHAR(36) FOREIGN KEY REFERENCES USERS (Id),
                                Blocked CHAR(36) FOREIGN KEY REFERENCES USERS (Id),
                                BlockDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                CONSTRAINT BLACKLIST_PK PRIMARY KEY (User, Blocked)
                            )";
            
            var tables = new string[] { usersTable, friendsTable, blockTable };

            using(var cmd = GetCommand())
            {
                foreach(var table in tables)
                {
                    cmd.CommandText = table;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static UserModel Find(string username)
        {
            EnsureDatabase();

            using var conn = GetConnection();
            
            var result = conn.Query<UserModel>("SELECT * FROM USERS WHERE UserName=:UserName AND FindInSearch=TRUE",
                                                new { UserName = username });
            return result.Count() == 0 ? null : result.First();
        }

        public static UserModel Get(string id)
        {
            EnsureDatabase();

            using var conn = GetConnection();
            
            var result = conn.Query<UserModel>("SELECT * FROM USERS WHERE ID=:Id", new { Id = id });
            return result.Count() == 0 ? null : result.First();
        }

        public static void Register(UserModel user)
        {
            using var conn = GetConnection();

            user.Id = Guid.NewGuid().ToString();

            try 
            {
                conn.Execute("INSERT INTO USERS VALUES @User", new { User = user });
            } catch (Npgsql.PostgresException ex)
            {
                var message = ex.ConstraintName ?? "" switch
                {
                    "USER_UNIQUE" => "Username already taken",
                    "EMAIL_UNIQUE" => "Email used by another account",
                    _ => ex.MessageText
                };

                throw new DataException(message);
            }
        }
    }
}