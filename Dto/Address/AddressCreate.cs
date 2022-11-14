using System.ComponentModel.DataAnnotations;

namespace Szpek.Application.Address
{
    public class AddressCreate
    {
        [Required]
        public string City { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string PostCode { get; set; }

        [Required]
        public string Voivodeship { get; set; }

        [Required]
        [StringLength(2)]
        public string CountryCode { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public int Height { get; set; }
    }
}
