using System;

namespace ChatServer.Exceptions
{
    public class ChatDataException : ChatBaseException
    {
        public ChatDataException(string Message = "") : base(Message) {}
    }
}