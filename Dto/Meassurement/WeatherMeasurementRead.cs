using System;

namespace Szpek.Application.Meassurement
{
    public class WeatherMeasurementRead
    {
        public long Id { get; }

        public double AtmosphericPreassure { get; }

        public double HumidityPercentage { get; }

        public DateTime MeasurementDate { get; }

        public WeatherMeasurementRead(long id, double atmosphericPreassure, double humidityPercentage, DateTime measurementDate)
        {
            Id = id;
            AtmosphericPreassure = atmosphericPreassure;
            HumidityPercentage = humidityPercentage;
            MeasurementDate = measurementDate;
        }
    }
}
