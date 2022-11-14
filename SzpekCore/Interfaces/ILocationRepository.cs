using System.Collections.Generic;
using System.Threading.Tasks;
using Szpek.Core.Models;

namespace Szpek.Core.Interfaces
{
    public interface ILocationRepository
    {
        Task Create(Location location);

        Task<IEnumerable<Location>> GetWithAdresses();

        Task<IEnumerable<Location>> GetActiveNonPrivate();

        Task<Location> Get(long id);

        Task<Location> GetInactive(long id);
        
        Task<Location> GetLocationWithAddressAndSensor(long id);

        Task<Location> GetActiveNonPrivateWithAddress(long locationId);

        //maybe remove and change to sensorId in code
        Task<Location> GetActiveBySensorCode(string sensorCode);

        Task<Location> GetActiveBySensorId(long sensorId);

        Task<Location> GetActiveWithAddressBySensorId(long sensorId);

        Task Update(Location oldLocation);

        //idk if it is the best place
        Task<bool> DoesBelongToSensorOwner(long locationId, long sensorOwnerId);
    }
}
