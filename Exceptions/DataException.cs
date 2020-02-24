using System;

namespace ChatServer.Exceptions
{
    public class DataException : BaseException
    {
        public DataException(string Message = "") : base(Message) {}
    }
}