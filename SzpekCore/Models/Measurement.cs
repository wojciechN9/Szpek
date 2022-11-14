namespace Szpek.Core.Models
{
    public class Measurement
    {
        public long Id { get; private set; }

        public long LocationId { get; private set; }

        public long SmogMeasurementId { get; private set; }

        public long? WeatherMeasurementId { get; private set; }

        public Location Location { get; private set; }

        public SmogMeasurement SmogMeasurement { get; private set; }

        public WeatherMeasurement WeatherMeasurement { get; private set; }

        public static Measurement Create(long locationId, SmogMeasurement smogMeasurement, WeatherMeasurement weatherMeasurement)
        {
            var meassurement = new Measurement(locationId, smogMeasurement, weatherMeasurement);

            return meassurement;
        }

        private Measurement(long locationId, SmogMeasurement smogMeasurement, WeatherMeasurement weatherMeasurement)
        {
            LocationId = locationId;
            SmogMeasurement = smogMeasurement;
            WeatherMeasurement = weatherMeasurement;
        }

        private Measurement() { }
    }
}
