using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Models.Requests
{
    public class CheckRequest
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
