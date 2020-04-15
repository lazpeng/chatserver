using ChatServer.Services.Interfaces;
using System;

namespace ChatServer.Services
{
    public class SecretProvider : ISecretProvider
    {
        private readonly string _secret;

        public SecretProvider()
        {
            _secret = Guid.NewGuid().ToString();
        }

        public string GetSecret() => _secret;
    }
}
