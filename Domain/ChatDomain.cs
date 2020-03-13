using ChatServer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Domain
{
    public class ChatDomain
    {
        private readonly IChatRepository _chatRepository;

        public ChatDomain(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }
    }
}
