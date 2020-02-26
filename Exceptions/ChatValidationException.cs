using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Exceptions
{
    public class ChatValidationException : ChatBaseException
    {
        public ChatValidationException(string Message = "") : base(Message)
        {

        }
    }
}
