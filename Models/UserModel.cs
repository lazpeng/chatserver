using System;

namespace ChatServer.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public DateTime? LastSeen { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime AccountCreated { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ProfilePicUrl { get; set; }
        public bool FindInSearch { get; set; }
        public bool OpenChat { get; set; }
    }
}