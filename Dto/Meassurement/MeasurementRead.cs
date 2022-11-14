namespace Szpek.Application.Meassurement
{
    public class MeasurementRead
    {
        public SmogMeasurementRead SmogMeasurement { get; }

        public WeatherMeasurementRead WeatherMeasurement{ get; }

        public MeasurementRead(SmogMeasurementRead smogMeasurement, WeatherMeasurementRead weatherMeasurement)
        {
            SmogMeasurement = smogMeasurement;
            WeatherMeasurement = weatherMeasurement;
        }
    }
}
