namespace Szpek.Application.Address
{
    public class AddressRead
    {
        public AddressRead(long id, string city, string street, string postCode, string voivodeship, string countryCode, double latitude, double longitude, int height)
        {
            Id = id;
            City = city;
            Street = street;
            PostCode = postCode;
            Voivodeship = voivodeship;
            CountryCode = countryCode;
            Latitude = latitude;
            Longitude = longitude;
            Height = height;
        }

        public long Id { get; }

        public string City { get; }

        public string Street { get; }

        public string PostCode { get; }

        public string Voivodeship { get; }

        public string CountryCode { get; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int Height { get; set; }
    }
}
