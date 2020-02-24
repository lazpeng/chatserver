using System;

namespace ChatServer.Exceptions
{
    public class BaseException : Exception
    {
        public BaseException(string Message = "") : base(Message) {}
    }
}