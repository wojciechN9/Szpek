using System.Collections.Generic;
using Szpek.Application.Address;
using Szpek.Application.Meassurement;

namespace Szpek.Application.Location
{
    public class LocationDetailsRead
    {
        public LocationDetailsRead(long id, bool isActive, long sensorId, string sensorCode, AddressRead address)
        {
            Id = id;
            IsActive = isActive;
            SensorId = sensorId;
            SensorCode = sensorCode;
            Address = address;
        }

        public long Id { get; }

        public bool IsActive { get; }

        public long SensorId { get; set; }

        public string SensorCode { get; set; }

        public AddressRead Address { get; }
    }
}
