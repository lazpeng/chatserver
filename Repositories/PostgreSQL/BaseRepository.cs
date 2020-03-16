using ChatServer.Repositories.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Reflection;
using System.IO;

namespace ChatServer.Repositories.PostgreSQL
{
    public class BaseRepository : IBaseRepository
    {
        protected readonly string ConnectionString;

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
                Database = databaseUri.LocalPath.TrimStart('/')
            };

            return builder.ToString();
        }

        public BaseRepository(string ConnectionString)
        {
            if (ConnectionString.StartsWith("postgres://"))
            {
                ConnectionString = FromHerokuConnectionString(ConnectionString);
            }
            this.ConnectionString = ConnectionString;
            PerformUpgrade();
        }

        public long LatestDatabaseVersion { get; } = 1;

        private string GetUpgradeScript(long version)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = $"ChatServer.Repositories.PostgreSQL.Scripts.v{version}.sql";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        private void UpdateDbVersion(long current)
        {
            using var conn = GetConnection();

            conn.Execute("INSERT INTO chat.DbUpgrade (Version, DateInstalled) VALUES (@current, CURRENT_DATE)", new { current });
        }

        public void PerformUpgrade()
        {
            long currentVersion = GetDatabaseVersion();

            try
            {
                while (currentVersion < LatestDatabaseVersion)
                {
                    var script = GetUpgradeScript(currentVersion);

                    using var conn = GetConnection();
                    conn.Execute(script);

                    UpdateDbVersion(currentVersion + 1);
                    currentVersion = GetDatabaseVersion();

                    Console.WriteLine($"Successfully performed upgrade to database version {currentVersion}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred when performing database upgrade: {e.Message}\n{e.StackTrace}");
            }
        }

        public long GetDatabaseVersion()
        {
            long version;

            try
            {
                using var cmd = GetCommand();
                cmd.CommandText = "SELECT MAX(Version) FROM chat.DbUpgrade";
                version = Convert.ToInt64(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                if (!(e is NpgsqlException))
                {
                    Console.WriteLine($"Something wrong occurred when checking database version: {e.Message}");
                }
                version = 0;
            }

            return version;
        }

        public IDbConnection GetConnection()
        {
            var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public IDbCommand GetCommand(string query = "", IDbConnection connection = null)
        {
            return new NpgsqlCommand(query, (connection ?? GetConnection()) as NpgsqlConnection);
        }

        public IDbDataParameter GetParameter(string name, object value)
        {
            return new NpgsqlParameter(name, value);
        }
    }
}
