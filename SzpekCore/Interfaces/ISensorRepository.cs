using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Szpek.Core.Models;

namespace Szpek.Core.Interfaces
{
    public interface ISensorRepository
    {
        Task Create(Sensor sensor);

        Task<IEnumerable<Sensor>> GetWithSensorOwnerAndLocations();

        Task<Sensor> Get(long id);

        Task<Sensor> Get(string code);

        Task<Sensor> GetWithSensorOwnerLocationAndAddress(long id);

        Task<Sensor> GetWithFirmware(long id);

        Task<Sensor> GetByCodeOrPublicKey(string sensorCode, string publicKey);

        Task Update(Sensor sensor);
    }
}
