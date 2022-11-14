using System;

namespace Szpek.Core.Models.Interfaces
{
    public interface IRequiredSmogMeassurementData
    {
        long Id { get;}

        AirQuality AirQuality { get; }

        double Pm10Value { get; }

        AirQuality Pm10Quality { get; }

        double Pm25Value { get; } //actually its PM2.5

        AirQuality Pm25Quality { get; }

        int SamplesQuantity { get; }

        DateTime PeriodFrom { get; }

        DateTime PeriodTo { get; }

        DateTime CreationDateTime { get; }

        Measurement Measurement { get; }
    }
}