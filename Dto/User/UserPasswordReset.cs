using System.ComponentModel.DataAnnotations;

namespace Szpek.Application.User
{
    public class UserPasswordReset
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
