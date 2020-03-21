using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Models.Requests
{
    public class EditUserRequest : UserModel
    {
        public string Token { get; set; }
    }
}
