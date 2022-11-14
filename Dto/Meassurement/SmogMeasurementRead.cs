using System;

namespace Szpek.Application.Meassurement
{
    public class SmogMeasurementRead
    {
        public SmogMeasurementRead(
            long id,
            AirQuality airQuality,
            double pm10Value,
            AirQuality pm10Quality,
            double pm25Value,
            AirQuality pm25Quality,
            double pm1Value,
            DateTime periodTo)
        {
            Id = id;
            AirQuality = airQuality;
            Pm10Value = pm10Value;
            Pm10Quality = pm10Quality;
            Pm25Value = pm25Value;
            Pm25Quality = pm25Quality;
            Pm1Value = pm1Value;
            PeriodTo = periodTo;
        }

        public long Id { get; }

        public AirQuality AirQuality { get; }

        public double Pm10Value { get; }

        public AirQuality Pm10Quality { get; }

        public double Pm25Value { get; }

        public AirQuality Pm25Quality { get; }

        public double Pm1Value { get; }

        public DateTime PeriodTo { get; }
    }
}
