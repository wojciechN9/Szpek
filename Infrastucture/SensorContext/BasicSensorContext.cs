using System;
using Szpek.Core.Models;

namespace Szpek.Infrastructure.SensorContext
{
    public class BasicSensorContext : ISensorContext
    {
        public bool IsIdentitySet { get; private set; } = false;

        private long _sensorId;
        public long SensorId
        {
            get { ThrowIfMissingIdentity(); return _sensorId; }
        }

        private string _sensorCode;
        public string SensorCode
        {
            get { ThrowIfMissingIdentity(); return _sensorCode; }
        }

        public void SetIdentity(Sensor sensor)
        {
            if (IsIdentitySet)
            {
                throw new InvalidOperationException("Sensor identity is already set for this request!");
            }

            _sensorId = sensor.Id;
            _sensorCode = sensor.Code;

            IsIdentitySet = true;
        }

        private void ThrowIfMissingIdentity()
        {
            if (!IsIdentitySet)
            {
                throw new Exception("Sensor identity missing!");
            }
        }
    }
}
