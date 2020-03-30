using ChatServer.Repositories.Interfaces;
using Npgsql;
using System;

namespace ChatServer.Repositories.PostgreSQL
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly string ConnectionString;

        private string FromHerokuConnectionString(string Conn)
        {
            var databaseUri = new Uri(Conn);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                Pooling = true,
                MinPoolSize = 1,
                MaxPoolSize = 20
            };

            return builder.ToString();
        }

        public ConnectionStringProvider(string ConnectionString)
        {
            if (ConnectionString.StartsWith("postgres://"))
            {
                ConnectionString = FromHerokuConnectionString(ConnectionString);
            }
            this.ConnectionString = ConnectionString;
        }

        public string GetConnectionString() => ConnectionString;
    }
}
