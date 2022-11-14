using System.Collections.Generic;
using Szpek.Application.Location;

namespace Szpek.Application.Sensor
{
    public class SensorDetailsRead
    {
        public SensorDetailsRead(long id, string code, bool isPrivate, OwnerRead owner, IEnumerable<LocationRead> locations)
        {
            Id = id;
            Code = code;
            IsPrivate = isPrivate;
            Owner = owner;
            Locations = locations;
        }

        public long Id { get; }

        public string Code { get; }

        public bool IsPrivate { get; }

        public OwnerRead Owner { get; }

        public IEnumerable<LocationRead> Locations { get; }
    }
}
