using System;

namespace ChatServer.Models
{
    public class AuthToken
    {
        public string UserId { get; set; }
        public DateTime Expiration { get; set; }
    }
}
