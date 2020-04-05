using System;

namespace ChatServer.Models
{
    public class BlockModel
    {
        public string BlockedUser { get; set; }
        public DateTime DateBlocked { get; set; }
    }
}
