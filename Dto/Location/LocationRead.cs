using Szpek.Application.Address;

namespace Szpek.Application.Location
{
    public class LocationRead
    {
        public LocationRead(long id, bool isActive, long sensorId, AddressRead address)
        {
            Id = id;
            IsActive = isActive;
            Address = address;
            SensorId = sensorId;
        }

        public long Id { get; }

        public bool IsActive { get; }

        public long SensorId { get; set; }

        public AddressRead Address { get; }
    }
}