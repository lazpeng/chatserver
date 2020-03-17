using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Models.Requests
{
    public class GetUserRequest
    {
        public string SourceId { get; set; }
        public string Token { get; set; }
    }
}
