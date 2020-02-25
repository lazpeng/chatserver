using System;
using System.Data;
using System.IO;
using Microsoft.Data.SqlClient;

namespace ChatServer.DAL
{
    public abstract class BaseDAL
    {
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
                    var path = Environment.CurrentDirectory;
                    return $"Data Source=(LocalDb)\\ChatServer;Integrated Security=True;";
                } else return $"Server={Host};Database={Database};User Id={User};Password={Password};";
            }
        }

        protected static IDbConnection GetConnection()
        {
            var conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        protected static IDbCommand GetCommand(string query = "", IDbConnection connection = null)
        {
            return new SqlCommand(query, (connection ?? GetConnection()) as SqlConnection);
        }

        protected static IDbDataParameter GetParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }

        protected static void EnsureSchema()
        {
            var query = @"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'chat')
                            BEGIN
                                EXEC('CREATE SCHEMA chat')
                            END";

            using var cmd = GetCommand(query);
            cmd.ExecuteNonQuery();
        }
    }
}