using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Exceptions
{
    public class ChatAuthException : ChatBaseException
    {
        public ChatAuthException(string Message = "") : base(Message)
        {

        }
    }
}
