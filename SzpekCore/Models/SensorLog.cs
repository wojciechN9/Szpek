using System;

namespace Szpek.Core.Models
{
    public class SensorLog
    {
        public int Id { get; set; }

        public long SensorId { get; set; }
        public Sensor Sensor { get; set; }

        public string Message { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public static SensorLog Create(long sensorId, string message)
        {
            var sensorLog = new SensorLog(sensorId, message);

            sensorLog.DateTimeUtc = DateTime.UtcNow;

            return sensorLog;
        }

        private SensorLog(long sensorId, string message)
        {
            SensorId = sensorId;
            Message = message;
        }
    }
}
