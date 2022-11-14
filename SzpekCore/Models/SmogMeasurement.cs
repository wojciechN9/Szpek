using System;
using System.Collections.Generic;
using System.Linq;
using Szpek.Core.Models.Interfaces;

namespace Szpek.Core.Models
{
    public class SmogMeasurement : IRequiredSmogMeassurementData
    {
        public long Id { get; private set; }

        public AirQuality AirQuality { get; private set; }

        public double Pm10Value { get; private set; }

        public AirQuality Pm10Quality { get; private set; }

        public double Pm25Value { get; private set; } //actually its PM2.5

        public AirQuality Pm25Quality { get; private set; }

        public double Pm1Value { get; private set; }

        public int SamplesQuantity { get; private set; }

        public DateTime PeriodFrom { get; private set; }

        public DateTime PeriodTo { get; private set; }

        public DateTime CreationDateTime { get; private set; }

        public Measurement Measurement { get; private set; }

        public static SmogMeasurement Create(double pm10Value, double pm25Value, int samplesQuantity, DateTime periodFrom, DateTime periodTo)
        {
            var meassurement = new SmogMeasurement(pm10Value, pm25Value, samplesQuantity, periodFrom, periodTo)
            {
                CreationDateTime = DateTime.UtcNow
            };

            return meassurement;
        }

        private SmogMeasurement(double pm10Value, double pm25Value, int samplesQuantity, DateTime periodFrom, DateTime periodTo)
        {
            Pm10Value = pm10Value;
            Pm25Value = pm25Value;
            SamplesQuantity = samplesQuantity;
            PeriodFrom = periodFrom;
            PeriodTo = periodTo; 
        }

        private SmogMeasurement() { }

        public void SetPm1Value(double? pm1Value) =>
            this.Pm1Value = pm1Value != null ? pm1Value.Value : this.Pm1Value = 0;

        public void SetAirQualities(IEnumerable<AirQualityLevel> airQualityLevels)
        {
            var pm10QualityLevels = airQualityLevels.Where(aql => aql.PollutionType == PollutionType.PM10);
            var pm25QualityLevels = airQualityLevels.Where(aql => aql.PollutionType == PollutionType.PM25);

            Pm10Quality = SetTypeOfQuality(pm10QualityLevels, Pm10Value);
            Pm25Quality = SetTypeOfQuality(pm25QualityLevels, Pm25Value);
            AirQuality = SetAirQuality(Pm10Quality, Pm25Quality);
        }

        private AirQuality SetTypeOfQuality(IEnumerable<AirQualityLevel> qualityLevels, double value)
        {
            qualityLevels = qualityLevels.OrderBy(ql => ql.MinValue);

            if (Convert.ToDecimal(value) == qualityLevels.First().MinValue)
            {
                return AirQuality.VeryGood;
            }

            foreach (var qualityLevel in qualityLevels)
            {
                if (Convert.ToDecimal(value) > qualityLevel.MinValue && Convert.ToDecimal(value) <= qualityLevel.MaxValue)
                {
                    return qualityLevel.AirQuality;
                }
            }

            //over 3000 is an error value
            return AirQuality.Error;
        }

        private AirQuality SetAirQuality(AirQuality pm10Quality, AirQuality pm25Quality)
        {
            return pm10Quality > pm25Quality ? pm10Quality : pm25Quality;
        }
    }
}
