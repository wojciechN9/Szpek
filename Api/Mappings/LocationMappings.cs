using System.Collections.Generic;
using System.Linq;
using Szpek.Application.Location;
using Szpek.Core.Models;

namespace Szpek.Api.Mappings
{
    public static class LocationMappings
    {
        public static IEnumerable<LocationRead> ToLocationsRead(this IEnumerable<Location> locations)
        {
            return locations.Select(l => ToLocationRead(l));
        }

        public static LocationRead ToLocationRead(this Location location)
        {
            if (location == null)
            {
                return null;
            }

            return new LocationRead(
                location.Id,
                location.IsActive,
                location.SensorId,
                location.Address.ToAddressRead());
        }

        public static LocationDetailsRead ToLocationDetailsRead(this Location location)
        {
            if (location == null)
            {
                return null;
            }

            return new LocationDetailsRead(
                        location.Id,
                        location.IsActive,
                        location.SensorId,
                        location.Sensor.Code,
                        location.Address.ToAddressRead());
        }

        public static LocationMeassurementsRead ToLocationMeassurementsRead(
            this Core.Models.Location location, 
            IEnumerable<Measurement> meassurements)
        {
            if (location == null || meassurements == null)
            {
                return null;
            }

            return new LocationMeassurementsRead(
                location.Id,
                location.IsActive, 
                location.Address.ToAddressRead(), 
                meassurements.ToMeasurementsRead());
        }
    }
}
