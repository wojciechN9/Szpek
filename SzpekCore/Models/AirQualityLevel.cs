
using System;

namespace Szpek.Core.Models
{
    public class AirQualityLevel
    {
        public long Id { get; private set; }

        public PollutionType PollutionType { get; private set; }

        public AirQuality AirQuality { get; private set; }

        public decimal MinValue { get; private set; }

        public decimal MaxValue { get; private set; } 

        public DateTime ModificationDateTime { get; private set; }

        //with id due to seeding data - do not wanna do reflections
        public AirQualityLevel(long id, PollutionType pollutionType, AirQuality airQuality, decimal minValue, decimal maxValue, DateTime modificationDateTime)
        {
            Id = id;
            PollutionType = pollutionType;
            AirQuality = airQuality;
            MinValue = minValue;
            MaxValue = maxValue;

            ModificationDateTime = modificationDateTime;
        }
    }
}
