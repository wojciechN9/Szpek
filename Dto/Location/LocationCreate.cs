using System.ComponentModel.DataAnnotations;
using Szpek.Application.Address;

namespace Szpek.Application.Location
{
    public class LocationCreate
    {
        [Required]
        public long SensorId { get; set; }

        [Required]
        public AddressCreate Address { get; set; }
    }
}
