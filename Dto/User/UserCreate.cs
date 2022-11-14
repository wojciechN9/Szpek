using System.ComponentModel.DataAnnotations;

namespace Szpek.Application.User
{
    public class UserCreate
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
