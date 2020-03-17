using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Models
{
    public class MessageModel
    {
        public long Id { get; set; }
        public string SourceId { get; set; }
        public string Content { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime? DateSeen { get; set; }
        public long? InReplyTo { get; set; }
    }
}
