using System.Collections.Generic;
using System.Linq;
using Szpek.Application.SensorOwner;
using Szpek.Core.Models;

namespace Szpek.Api.Mappings
{
    public static class SensorOwnerMappings
    {
        public static IEnumerable<SensorOwnerRead> ToSensorsOwnersRead(this IEnumerable<SensorOwner> sensorsOwners)
        {
            return sensorsOwners.Select(s => s.ToSensorOwnerRead());
        }

        public static SensorOwnerRead ToSensorOwnerRead(this SensorOwner sensorOwner)
        {
            if (sensorOwner != null)
            {
                return new SensorOwnerRead(
                    sensorOwner.Id,
                    sensorOwner.Name, 
                    sensorOwner.Address, 
                    sensorOwner.IsCompany, 
                    sensorOwner.Sensors.ToSensorRead());
            }

            return null;
        }
    }
}
