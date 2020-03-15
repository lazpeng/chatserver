using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Exceptions
{
    public class ChatPermissionException : ChatBaseException
    {
        public ChatPermissionException(string Message = "") : base(Message)
        {

        }
    }
}
