using ChatServer.Domain.Interfaces;
using ChatServer.Models.Responses;
using ChatServer.Repositories.Interfaces;
using ChatServer.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Models.Requests;
using ChatServer.Exceptions;

namespace ChatServer.Domain
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUserService _userDomain;
        private readonly TimeSpan TokenValidity = TimeSpan.FromHours(12);

        public AuthService(IAuthRepository authRepository, IUserService userDomain)
        {
            _authRepository = authRepository;
            _userDomain = userDomain;
        }

        private async Task<bool> IsTokenValid(string UserId, string Token)
        {
            return (await _authRepository.GetValidTokensForUser(UserId)).Contains(Token);
        }

        public async Task Authorize(BaseAuthenticatedRequest Request)
        {
            if(! await IsTokenValid(Request.SourceId, Request.Token))
            {
                throw new ChatAuthException();
            }
        }

        public async Task<LoginResponse> PerformLogin(string UserName, string Password, bool AppearOffline)
        {
            await _authRepository.DeleteExpiredSessions();

            var users = await _userDomain.Search(UserName, false);
            if(users.Any())
            {
                var user = users.First();

                var hashAndSalt = await _authRepository.GetPasswordHashAndSalt(user.Id);

                var calculated = HashHelper.CalculateHash(Password, hashAndSalt.Item2);

                if (calculated == hashAndSalt.Item1)
                {
                    var token = Guid.NewGuid().ToString();

                    await _authRepository.SaveNewToken(user.Id, token, TokenValidity);
                    if(!AppearOffline)
                    {
                        await _userDomain.UpdateLastLogin(user.Id);
                        await _userDomain.UpdateLastSeen(user.Id);
                    }

                    return new LoginResponse
                    {
                        ErrorMessage = "",
                        Success = true,
                        Token = token,
                        Id = user.Id
                    };
                }
            }

            return new LoginResponse
            {
                ErrorMessage = "No such username/password combination",
                Success = false,
                Token = null,
                Id = null
            };
        }
    }
}
