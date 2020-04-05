using ChatServer.Models;
using ChatServer.Repositories.Interfaces;
using ChatServer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatServer.Services
{
    public class FriendService : IFriendService
    {
        private readonly IFriendRepository _friendRepository;
        private readonly IBlockService _blockService;

        public FriendService(IFriendRepository friendRepository, IBlockService blockService)
        {
            _friendRepository = friendRepository;
            _blockService = blockService;
        }

        public async Task AnswerFriendRequest(long Id, bool Accepted)
        {
            await _friendRepository.AnswerFriendRequest(Id, Accepted);
        }

        public async Task<bool> AreUsersFriends(string A, string B)
        {
            return await _friendRepository.AreUsersFriends(A, B);
        }

        public async Task DeleteFriendRequest(long Id)
        {
            await _friendRepository.DeleteFriendRequest(Id);
        }

        public async Task<List<FriendModel>> GetFriendList(string User)
        {
            return await _friendRepository.GetFriendList(User);
        }

        public async Task<List<FriendRequestModel>> GetFriendRequests(string User)
        {
            return await _friendRepository.GetFriendRequests(User);
        }

        public async Task RemoveFriend(string A, string B)
        {
            await _friendRepository.RemoveFriend(A, B);
        }

        public async Task<FriendRequestModel> SendFriendRequest(string SourceId, string TargetId)
        {
            Console.WriteLine(await AreUsersFriends(SourceId, TargetId));
            Console.WriteLine(await _blockService.IsUserBlocked(SourceId, TargetId));
            Console.WriteLine(await _friendRepository.HasPendingRequest(SourceId, TargetId));

            FriendRequestModel result = null;
            if(!await AreUsersFriends(SourceId, TargetId) &&
               !await _blockService.IsUserBlocked(SourceId, TargetId) &&
               !await _friendRepository.HasPendingRequest(SourceId, TargetId))
            {
                result = await _friendRepository.SendFriendRequest(SourceId, TargetId);
            } else if(await _friendRepository.HasPendingRequest(TargetId, SourceId))
            {
                var request = await _friendRepository.FindRequest(TargetId, SourceId);
                await AnswerFriendRequest(request.Id, true);
                result = await _friendRepository.GetRequest(request.Id);
            }

            return result;
        }
    }
}
