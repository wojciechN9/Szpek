namespace Szpek.Core.Models
{
    public class Address
    {
        public long Id { get; private set; }

        public string City { get; private set; }

        public string Street { get; private set; }

        public string PostCode { get; private set; }

        public string Voivodeship { get; private set; }

        public string CountryCode { get; private set; }

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public int Height { get; private set; }

        public long LocationId { get; private set; }

        public Location Location { get; private set; }

        public static Address Create(string city, string street, string postCode, string voivodeship, string countryCode, double latitude, double longitude, int height)
        {
            return new Address(city, street, postCode, voivodeship, countryCode, latitude, longitude, height);
        }

        private Address(string city, string street, string postCode, string voivodeship, string countryCode, double latitude, double longitude, int height)
        {
            City = city;
            Street = street;
            PostCode = postCode;
            Voivodeship = voivodeship;
            CountryCode = countryCode;
            Latitude = latitude;
            Longitude = longitude;
            Height = height;
        }

        internal void ChangeStreetAddress(string streetAddress)
        {
            Street = streetAddress;
        }
    }
}
