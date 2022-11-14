using System.ComponentModel.DataAnnotations;

namespace Szpek.Application.User
{
    public class UserLogin
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
