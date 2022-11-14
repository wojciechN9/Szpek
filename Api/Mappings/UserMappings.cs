using System.Collections.Generic;
using System.Linq;
using Szpek.Application.User;

namespace Szpek.Api.Mappings
{
    public static class UserMappings
    {
        public static IEnumerable<UserRead> ToUsersRead(this IEnumerable<Core.Models.User> users)
        {
            return users.Select(s => ToUserRead(s));
        }

        public static UserRead ToUserRead(this Core.Models.User user)
        {
            if (user != null)
            {
                return new UserRead(user.Id, user.UserName, user.Email, user.SensorOwnerId);
            }

            return null;
        }
    }
}
