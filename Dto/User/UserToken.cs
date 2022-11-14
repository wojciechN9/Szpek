using System.Collections.Generic;

namespace Szpek.Application.User
{
    public class UserToken
    {
        public string Username { get; }

        public IEnumerable<string> Roles { get; }

        public string Token { get; }

        public UserToken(string username, IEnumerable<string> roles, string token)
        {
            Username = username;
            Roles = roles;
            Token = token;
        }
    }
}
