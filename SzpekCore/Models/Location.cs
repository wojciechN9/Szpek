using System.Collections.Generic;

namespace Szpek.Core.Models
{
    public class Location
    {
        public long Id { get; private set; }

        public bool IsActive { get; private set; }

        public long SensorId { get; private set; }

        public Sensor Sensor { get; private set; }

        public Address Address { get; private set; }

        public List<Measurement> Meassurements { get; private set; }

        private Location() { }

        public static Location Create(long sensorId, Address address)
        {
            var location = new Location(sensorId, address);
            location.Activate();

            return location;
        }

        private Location(long sensorId, Address address)
        {
            SensorId = sensorId;
            Address = address;
        }

        public void Activate(long sensorId)
        {
            this.SensorId = sensorId;
            this.Activate();
        }

        private void Activate()
        {
            this.IsActive = true;
        }

        public void Deactivate()
        {
            this.IsActive = false;
        }

        public void ChangeStreetAddress(string streetAddress)
        {
            Address.ChangeStreetAddress(streetAddress);
        }
    }
}
