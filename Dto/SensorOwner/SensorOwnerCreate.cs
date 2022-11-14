using System.ComponentModel.DataAnnotations;

namespace Szpek.Application.SensorOwner
{
    public class SensorOwnerCreate
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Address { get; set; }

        [Required]
        public bool IsCompany { get; set; }
    }
}
