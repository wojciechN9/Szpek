using System.Collections.Generic;

namespace Szpek.Core.Models
{
    public class SensorOwner
    {
        public long Id { get; set; }

        public string Name { get; private set; }

        public string Address { get; private set; }

        public bool IsCompany { get; private set; }

        public List<Sensor> Sensors { get; private set; }

        public static SensorOwner Create(string name, bool isCompany, string address)
        {
            return new SensorOwner(name, isCompany, address);
        }

        private SensorOwner(string name, bool isCompany, string address)
        {
            Name = name;
            IsCompany = isCompany;
            Address = address;
        }

        public void UpdateAddress(string address)
        {
            Address = address ?? "";
        }
    }
}
