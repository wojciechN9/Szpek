using System.ComponentModel.DataAnnotations;

namespace Szpek.Application.Sensor
{
    public class SensorCreate
    {
        [Required]
        [StringLength(45)]
        public string Code { get; set; }

        [Required]
        public string PublicKey { get; set; }

        [Required]
        public long OwnerId { get; set; }

        [Required]
        public bool IsPrivate { get; set; }
    }
}
