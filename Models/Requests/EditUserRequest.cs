using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Models.Requests
{
    public class EditUserRequest : BaseAuthenticatedRequest
    {
        public UserModel User { get; set; }
    }
}
