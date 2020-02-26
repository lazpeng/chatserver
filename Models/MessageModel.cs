using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Models
{
    public class MessageModel
    {
        public ulong MessageId { get; set; }
        public string FromId { get; set; }
        public string Content { get; set; }
        public DateTime DateSent { get; set; }
        public ulong? InReplyTo { get; set; }
    }
}
