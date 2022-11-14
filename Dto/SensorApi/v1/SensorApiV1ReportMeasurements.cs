using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Szpek.Application.SensorApi.v1
{
    public class SensorApiV1ReportMeasurements
    {
        [Required]
        public double Pm10Value { get; set; }

        [Required]
        public double Pm2_5Value { get; set; }

        [Required]
        public double Pm1Value { get; set; }

        [Required]
        public int SamplesCount { get; set; }

        [Required]
        public long TimestampFrom { get; set; }

        [Required]
        public long TimestampTo { get; set; }

        [Required]
        public double TemperatureCelsius { get; set; }

        [Required]
        public double PressureHPa { get; set; }

        [Required]
        public double HumidityPercent { get; set; }
    }
}
