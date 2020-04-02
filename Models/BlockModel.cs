using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Models
{
    public class BlockModel
    {
        public long Id { get; set; }
        public string BlockedUser { get; set; }
        public DateTime DateBlocked { get; set; }
    }
}
