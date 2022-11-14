using System.Collections.Generic;
using Szpek.Application.Sensor;

namespace Szpek.Application.SensorOwner
{
    public class SensorOwnerRead
    {
        public SensorOwnerRead(long id, string name, string address, bool isCompany, IEnumerable<SensorRead> sensors)
        {
            Id = id;
            Name = name;
            Address = address;
            IsCompany = isCompany;
            Sensors = sensors;
        }

        public long Id { get; }

        public string Name { get; }

        public string Address { get; }

        public bool IsCompany { get; }

        public IEnumerable<SensorRead> Sensors { get; }
    }
}
