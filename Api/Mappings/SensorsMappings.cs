using System.Collections.Generic;
using System.Linq;
using Szpek.Application.Sensor;

namespace Szpek.Api.Mappings
{
    public static class SensorsMappings
    {
        public static IEnumerable<SensorRead> ToSensorRead(this IEnumerable<Core.Models.Sensor> sensor)
        {
            return sensor.Select(s => ToSensorRead(s));
        }

        public static SensorDetailsRead ToSensorDetailsRead(this Core.Models.Sensor sensor)
        {
            if (sensor != null)
            {
                return new SensorDetailsRead(
                    sensor.Id,
                    sensor.Code,
                    sensor.IsPrivate,
                    sensor.SensorOwner.ToOwnerRead(),
                    sensor.Locations.ToLocationsRead());
            }

            return null;
        }

        public static SensorRead ToSensorRead(this Core.Models.Sensor sensor)
        {
            if (sensor != null)
            {
                return new SensorRead(
                    sensor.Id,
                    sensor.Code,
                    sensor.IsPrivate,
                    sensor.Locations?.FirstOrDefault(l => l.IsActive == true)?.Id,
                    sensor.SensorOwner.ToOwnerRead());
            }

            return null;
        }

        public static OwnerRead ToOwnerRead(this Core.Models.SensorOwner sensorOwner)
        {
            if (sensorOwner != null)
            {
                return new OwnerRead(sensorOwner.Id, sensorOwner.Name);
            }

            return null;
        }
    }
}
