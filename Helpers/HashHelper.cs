using System;
using System.Security.Cryptography;
using System.Text;

namespace ChatServer.Helpers
{
    public abstract class HashHelper
    {
        public static string CalculateHash(string password, string salt)
        {
            using var algo = new SHA256Managed();
            var finalBytes = Encoding.UTF8.GetBytes(password + salt);
            return Convert.ToBase64String(algo.ComputeHash(finalBytes));
        }
    }
}
