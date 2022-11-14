using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.Models.Context;

namespace Szpek.Data.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        private readonly SzpekContext _context;

        public SensorRepository(SzpekContext szpekContext) => _context = szpekContext;

        public async Task Create(Sensor sensor)
        {
            await _context.AddAsync(sensor);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Sensor>> GetWithSensorOwnerAndLocations()
        {
            return await _context.Sensor
                .Include(s => s.SensorOwner)
                .Include(s => s.Locations)
                .ToListAsync();
        }

        public async Task<Sensor> Get(long id)
        {
            return await _context.Sensor
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sensor> Get(string code)
        {
            return await _context.Sensor
               .SingleOrDefaultAsync(s => s.Code == code);
        }

        public async Task<Sensor> GetWithSensorOwnerLocationAndAddress(long id)
        {
            return await _context.Sensor
                .Include(s => s.SensorOwner)
                .Include(s => s.Locations).ThenInclude(l => l.Address)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sensor> GetWithFirmware(long id)
        {
            return await _context.Sensor
                .Include(s => s.InstalledFirmware)
                .Include(s => s.RecommendedFirmware)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sensor> GetByCodeOrPublicKey(string sensorCode, string publicKey)
        {
            return await _context.Sensor
                .SingleOrDefaultAsync(s => s.Code == sensorCode || s.PublicKey == publicKey);
        }

        public async Task Update(Sensor sensor)
        {
            _context.Update(sensor);
            await _context.SaveChangesAsync();
        }
    }
}
