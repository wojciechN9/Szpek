using Szpek.Application.Address;

namespace Szpek.Api.Mappings
{
    public static class AddressMappings
    {
        public static AddressRead ToAddressRead(this Core.Models.Address address)
        {
            if (address != null)
            {
                return new AddressRead(address.Id, address.City, address.Street, address.PostCode, address.Voivodeship, address.CountryCode, address.Latitude, address.Longitude, address.Height);
            }

            return null;
        }
    }
}
