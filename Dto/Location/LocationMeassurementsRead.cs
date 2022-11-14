using System.Collections.Generic;
using Szpek.Application.Address;
using Szpek.Application.Meassurement;

namespace Szpek.Application.Location
{
    public class LocationMeassurementsRead
    {
        public LocationMeassurementsRead(long id, bool isActive, AddressRead address)
        {
            Id = id;
            IsActive = isActive;
            Address = address;

            Meassurements = new List<MeasurementRead>();
        }

        public LocationMeassurementsRead(
            long id,
            bool isActive,
            AddressRead address,
            IEnumerable<MeasurementRead> meassurements) : this(id, isActive, address)
        {
            Meassurements = meassurements;
        }

        public long Id { get; }

        public bool IsActive { get; }

        public AddressRead Address { get; }

        public IEnumerable<MeasurementRead> Meassurements { get; }
    }
}
