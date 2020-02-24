using System;

namespace ChatServer.Models.Requests
{
    public class LoginRequest
    {
        public ulong Id { get; set; }
        public string Password { get; set; }
        public bool AppearOffline { get; set; }
    }
}