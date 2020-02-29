using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Models
{
    public class FriendModel
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public DateTime? FriendsSince { get; set; }
        public DateTime RequestSent { get; set; }
    }
}
