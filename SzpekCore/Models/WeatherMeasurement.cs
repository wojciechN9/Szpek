using System;

namespace Szpek.Core.Models
{
    public class WeatherMeasurement
    {
        private const double MollarMassOfAir = 0.0289644; // kg/mol
        private const double GravitationalAcceleration = 9.80665; // m/(s^2)
        private const double UniversalGasConstant = 8.3144598; // J/(mol*K)

        public long Id { get; private set; }

        public double CelciusTemperature { get; private set; }

        public double AtmosphericPreassure { get; private set; } // typo, should be Pressure

        public double HumidityPercentage { get; private set; }

        public DateTime MeasurementDate { get; private set; }

        public DateTime CreationDateTime { get; private set; }

        public Measurement Measurement { get; private set; }

        public static WeatherMeasurement Create(double temperature, double pressure, double humidity, DateTime measurementDate)
        {
            return new WeatherMeasurement()
            {
                CelciusTemperature = temperature,
                AtmosphericPreassure = pressure,
                HumidityPercentage = humidity,
                MeasurementDate = measurementDate,
                CreationDateTime = DateTime.UtcNow
            };
        }

        public void CalculateBarometricFormula(int height)
        {
            var kelvinTemperature = this.CelciusTemperature + 273.15;

            this.AtmosphericPreassure = this.AtmosphericPreassure
                / Math.Exp((-1 * MollarMassOfAir * GravitationalAcceleration * height) / (UniversalGasConstant * kelvinTemperature));
        }
    }
}
