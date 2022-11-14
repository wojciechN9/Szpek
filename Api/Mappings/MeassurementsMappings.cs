using System.Collections.Generic;
using System.Linq;
using Szpek.Application.Meassurement;

namespace Szpek.Api.Mappings
{
    public static class MeassurementsMappings
    {
        public static IEnumerable<MeasurementRead> ToMeasurementsRead(this IEnumerable<Core.Models.Measurement> meassurements)
            => meassurements.Select(m => ToMeasurementRead(m));

        public static MeasurementRead ToMeasurementRead(this Core.Models.Measurement measurement)
        {
            if(measurement == null)
            {
                return null;
            }

            return new MeasurementRead(
                measurement.SmogMeasurement.ToSmogMeasurementRead(),
                measurement.WeatherMeasurement.ToWeatherMeasurementRead());
        }

        public static SmogMeasurementRead ToSmogMeasurementRead(this Core.Models.SmogMeasurement smogMeassurement)
        {
            if (smogMeassurement == null)
            {
                return null;
            }

            return new SmogMeasurementRead(
                smogMeassurement.Id,
                (AirQuality)smogMeassurement.AirQuality,
                smogMeassurement.Pm10Value,
                (AirQuality)smogMeassurement.Pm10Quality,
                smogMeassurement.Pm25Value,
                (AirQuality)smogMeassurement.Pm25Quality,
                smogMeassurement.Pm1Value,
                smogMeassurement.PeriodTo);
        }

        public static WeatherMeasurementRead ToWeatherMeasurementRead(this Core.Models.WeatherMeasurement weatherMeasurement)
        {
            if (weatherMeasurement == null)
            {
                return null;
            }

            return new WeatherMeasurementRead(
                weatherMeasurement.Id,
                weatherMeasurement.AtmosphericPreassure,
                weatherMeasurement.HumidityPercentage,
                weatherMeasurement.MeasurementDate);
        }
    }
}
