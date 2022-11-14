using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Szpek.Application.SensorApi.v1
{
    public class SensorApiV1ReportSmog
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
    }
}
