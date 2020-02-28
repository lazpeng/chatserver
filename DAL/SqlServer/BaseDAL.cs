using System;
using System.Data;
using System.IO;
using System.Reflection;
using ChatServer.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using Dapper;

namespace ChatServer.DAL.SqlServer
{
    public abstract class BaseDAL : IBaseDAL
    {
        public BaseDAL()
        {
            PerformUpgrade();
        }

        private static string Host
        {
            get
            {
                return Environment.GetEnvironmentVariable("CHAT_DB_HOST");
            }
        }

        private static string User
        {
            get
            {
                return Environment.GetEnvironmentVariable("CHAT_DB_USER");
            }
        }

        private static string Password
        {
            get
            {
                return Environment.GetEnvironmentVariable("CHAT_DB_PASS");
            }
        }

        private static string Database
        {
            get
            {
                return Environment.GetEnvironmentVariable("CHAT_DB_NAME");
            }
        }

        private static string ConnectionString
        {
            get
            {
                if(string.IsNullOrEmpty(Host))
                {
                    return $"Data Source=(LocalDb)\\ChatServer;Integrated Security=True;";
                } else return $"Server={Host};Database={Database};User Id={User};Password={Password};";
            }
        }

        public long LatestDatabaseVersion { get; } = 1;

        private string GetUpgradeScript(long version)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = $"ChatServer.DAL.SqlServer.Scripts.v{version}.sql";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        public void PerformUpgrade()
        {
            long currentVersion = GetDatabaseVersion();

            try
            {
                while (currentVersion < LatestDatabaseVersion)
                {
                    var script = GetUpgradeScript(currentVersion);

                    var batches = script.Split("GO");

                    using var conn = GetConnection();

                    foreach(var batch in batches)
                    {
                        conn.Execute(batch);
                    }

                    currentVersion = GetDatabaseVersion();
                }
            } catch (Exception e)
            {
                // TODO: Better log exception
                Console.WriteLine($"An error occurred when performing database upgrade: {e.Message}\n{e.StackTrace}");
            }
        }

        public long GetDatabaseVersion()
        {
            long version;

            try
            {
                using var cmd = GetCommand();
                cmd.CommandText = "SELECT MAX(Version) FROM chat.DBINFO";
                version = Convert.ToInt64(cmd.ExecuteScalar());
            } catch (Exception e)
            {
                if(!(e is SqlException))
                {
                    Console.WriteLine($"Something wrong occurred when checking database version: {e.Message}");
                }
                version = 0;
            }

            return version;
        }

        public IDbConnection GetConnection()
        {
            var conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public IDbCommand GetCommand(string query = "", IDbConnection connection = null)
        {
            return new SqlCommand(query, (connection ?? GetConnection()) as SqlConnection);
        }

        public IDbDataParameter GetParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }
    }
}