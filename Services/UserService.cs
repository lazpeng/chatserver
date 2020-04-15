using ChatServer.Services.Interfaces;
using ChatServer.Models;
using ChatServer.Repositories.Interfaces;
using ChatServer.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using ChatServer.Helpers;
using ChatServer.Models.Requests;

namespace ChatServer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFriendService _friendService;

        public UserService(IUserRepository userRepository, IFriendService friendService)
        {
            _userRepository = userRepository;
            _friendService = friendService;
        }

        public async Task<UserModel> Register(UserModel user)
        {
            user.DataHash = GenerateSalt();
            var result = await _userRepository.Register(user);
            await UpdateUserPassword(user.Id, user.Password);

            return result;
        }

        public async Task<UserModel> Get(string FromId, string Id)
        {
            return await _userRepository.Get(Id);
        }

        public async Task<List<UserModel>> Search(string Username, bool Partial = true)
        {
            return await _userRepository.Find(Username, Partial);
        }

        public async Task DeleteAccount(string UserId)
        {
            await _userRepository.DeleteAccount(UserId);
        }

        public async Task UpdateLastLogin(string UserId)
        {
            await _userRepository.UpdateLastLogin(UserId);
        }

        public async Task UpdateLastSeen(string UserId)
        {
            await _userRepository.UpdateLastSeen(UserId);
        }

        private string GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var buffer = new byte[32];
            rng.GetBytes(buffer);

            return Convert.ToBase64String(buffer);
        }

        public async Task UpdateUserPassword(string UserId, string Password)
        {
            var newSalt = GenerateSalt();
            var newHash = HashHelper.CalculateHash(Password, newSalt);

            await _userRepository.SavePasswordHash(UserId, newHash, newSalt);
        }

        public async Task Edit(string Id, UserModel User)
        {
            User.Id = Id;
            User.DataHash = GenerateSalt();

            await _userRepository.Edit(User);

            if(! string.IsNullOrEmpty(User.Password))
            {
                if(User.Password.Length < 6)
                {
                    throw new ChatBaseException("Password length must be >= 6 characters");
                }

                await UpdateUserPassword(Id, User.Password);
            }
        }

        public async Task<Tuple<string, string>> GetPasswordHashAndSalt(string UserId)
        {
            return await _userRepository.GetPasswordHashAndSalt(UserId);
        }

        public async Task<List<string>> CheckUsersUpdate(CheckUserUpdateRequest Request)
        {
            var outdated = new List<string>();

            foreach(var user in Request.KnownUsers)
            {
                if(! await _userRepository.IsUserUpToDate(user.UserId, user.LastDataHash))
                {
                    outdated.Add(user.UserId);
                }
            }

            return outdated;
        }
    }
}
