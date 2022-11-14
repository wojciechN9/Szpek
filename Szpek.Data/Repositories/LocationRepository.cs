using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.Models.Context;

namespace Szpek.Data.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly SzpekContext _context;

        public LocationRepository(SzpekContext szpekContext)
        {
            _context = szpekContext;
        }

        public async Task Create(Location location)
        {
            await _context.AddAsync(location);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Location>> GetWithAdresses()
        {
            return await _context.Location
                .Include(l => l.Address)
                .ToListAsync();
        }

        public async Task<IEnumerable<Location>> GetActiveNonPrivate()
        {
            return await _context.Location
               .Include(l => l.Address)
               .Where(l => l.IsActive == true)
               .Where(l => l.Sensor.IsPrivate == false)
               .ToListAsync();
        }

        public async Task<Location> Get(long id)
        {
            return await _context.Location
                      .Where(l => l.Id == id)
                      .SingleOrDefaultAsync();
        }

        public async Task<Location> GetInactive(long id)
        {
           return await _context.Location.SingleOrDefaultAsync(
                l => l.IsActive == false && l.Id == id);
        }

        public async Task<Location> GetLocationWithAddressAndSensor(long id)
        {
            return await _context.Location
                      .Include(l => l.Address)
                      .Include(l => l.Sensor)
                      .Where(l => l.Id == id)
                      .SingleOrDefaultAsync();
        }

        public async Task<Location> GetActiveNonPrivateWithAddress(long id)
        {
            return await _context.Location
                .Include(l => l.Address)
                .Where(l => l.Id == id && l.IsActive == true)
                .Where(l => l.Sensor.IsPrivate == false)
                .SingleOrDefaultAsync();
        }

        public async Task<Location> GetActiveBySensorCode(string sensorCode)
        {
            return await _context.Location
                .Where(l => l.Sensor.Code == sensorCode && l.IsActive == true)
                .SingleOrDefaultAsync();
        }

        public async Task<Location> GetActiveBySensorId(long sensorId)
        {
            return await _context.Location
                .SingleOrDefaultAsync(l => l.IsActive == true && l.SensorId == sensorId);
        }

        public async Task<Location> GetActiveWithAddressBySensorId(long sensorId)
        {
            return await _context.Location
                .Include(l => l.Address)
                .SingleOrDefaultAsync(l => l.IsActive == true && l.SensorId == sensorId);
        }

        public async Task Update(Location oldLocation)
        {
            _context.Update(oldLocation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DoesBelongToSensorOwner(long locationId, long sensorOwnerId)
        {
            var locationSensorId = (await _context.Location.Where(l => l.Id == locationId).FirstOrDefaultAsync()).SensorId;

            var sensorOwnerSensorsIds = await _context.Sensor
                .Where(s => s.SensorOwnerId == sensorOwnerId)
                .Select(s => s.Id).ToListAsync();

            return sensorOwnerSensorsIds.Contains(locationSensorId);
        }
    }
}
