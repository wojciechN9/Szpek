using System;
using System.ComponentModel.DataAnnotations;

namespace Szpek.Application.Meassurement
{
    public class MeassurementCreate
    {
        [Required]
        public double Pm10Value { get; set; }

        [Required]
        public double Pm25Value { get; set; }

        public double Pm1Value { get; set; }

        [Required]
        public int SamplesQuantity { get; set; }

        [Required]
        public DateTime PeriodFrom { get; set; }

        [Required]
        public DateTime PeriodTo { get; set; }

        [Required]
        public string SensorCode { get; set; }
    }
}
