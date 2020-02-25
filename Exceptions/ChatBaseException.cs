using System;

namespace ChatServer.Exceptions
{
    public class ChatBaseException : Exception
    {
        public ChatBaseException(string Message = "") : base(Message) {}
    }
}