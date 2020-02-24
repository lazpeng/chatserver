using System;
using System.Data;
using Npgsql;

namespace ChatServer.DAL
{
    public abstract class BaseDAL
    {
        private static string Host
        {
            get
            {
                return Environment.GetEnvironmentVariable("CHAT_DB_HOST") ?? "localhost";
            }
        }

        private static string User
        {
            get
            {
                return Environment.GetEnvironmentVariable("CHAT_DB_USER") ?? "postgres";
            }
        }

        private static string Password
        {
            get
            {
                return Environment.GetEnvironmentVariable("CHAT_DB_PASS") ?? "postgres";
            }
        }

        private static string Database
        {
            get
            {
                return Environment.GetEnvironmentVariable("CHAT_DB_NAME") ?? "chat";
            }
        }

        private static string Port
        {
            get
            {
                return Environment.GetEnvironmentVariable("CHAT_DB_PORT") ?? "5432";
            }
        }

        private static string ConnectionString
        {
            get
            {
                return $"Server={Host};Port={Port};Database={Database};User Id={User};Password={Password};";
            }
        }

        protected static IDbConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        protected static IDbCommand GetCommand(string query = "", IDbConnection connection = null)
        {
            return new NpgsqlCommand(query, (connection ?? GetConnection()) as NpgsqlConnection);
        }

        protected static IDbDataParameter GetParameter(string name, object value)
        {
            return new NpgsqlParameter(name, value);
        }
    }
}