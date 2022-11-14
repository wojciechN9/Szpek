using System.Collections.Generic;
using System.Threading.Tasks;
using Szpek.Core.Models;

namespace Szpek.Core.Interfaces
{
    public interface ISensorOwnerRepository
    {
        Task Create(SensorOwner sensorOwner);

        Task<IEnumerable<SensorOwner>> GetWithSensors();

        Task<SensorOwner> Get(long id);

        Task<SensorOwner> GetWithSensors(long id);

        Task<SensorOwner> GetWithSensorsAndLocations(long id);

        Task<long> GetIdFromHttpContextUser(string userId);
    }
}
