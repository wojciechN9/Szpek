using System.ComponentModel.DataAnnotations;

namespace Szpek.Application.User
{
    public class UserRemindPassword
    {
        [Required]
        public string Email { get; set; }
    }
}
