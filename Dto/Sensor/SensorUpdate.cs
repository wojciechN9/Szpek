using System.ComponentModel.DataAnnotations;

namespace Szpek.Application.Sensor
{
    public class SensorUpdate
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public bool IsPrivate { get; set; }
    }
}
