using System;

namespace ChatServer.Models.Responses
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public ulong UserId { get; set; }
    }
}