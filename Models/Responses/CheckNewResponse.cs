using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Models.Responses
{
    public class CheckNewResponse
    {
        public List<MessageModel> NewMessages { get; set; }
        public List<MessageModel> EditedMessages { get; set; }
        public List<long> DeletedMessages { get; set; }
    }
}
