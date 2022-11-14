using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.Models.Context;

namespace Szpek.Data.Repositories
{
    public class MeassurementRepository : IMeassurementRepository
    {
        private readonly SzpekContext _context;

        public MeassurementRepository(SzpekContext szpekContext) => _context = szpekContext;

        public async Task Create(Measurement meassurement)
        {
            await _context.AddAsync(meassurement);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Measurement>> GetDescForSpecificDate(long locationId, DateTime date)
        {
            return await _context.Measurement
                    .Include(m => m.SmogMeasurement).Include(m => m.WeatherMeasurement)
                    .Where(m => m.LocationId == locationId && m.SmogMeasurement.PeriodTo.Date == date.Date)
                    .OrderByDescending(m => m.SmogMeasurement.PeriodTo)
                    .ToListAsync();
        }

        public async Task<IEnumerable<Measurement>> GetLatests(IEnumerable<long> locationsIds)
        {
            var yesterdaySameTime = DateTime.UtcNow.AddDays(-1);

            //.Select( .First() ) inside GroupBy currently not working in ef core (v3.1)
            var last24hMeassurements = await _context.Measurement
                .Include(m => m.SmogMeasurement)
                .Where(m => m.SmogMeasurement.PeriodTo > yesterdaySameTime)
                .Where(m => locationsIds.Contains(m.LocationId))
                .OrderByDescending(m => m.SmogMeasurement.PeriodTo)
                .ToListAsync();

            return last24hMeassurements
                .GroupBy(m => m.LocationId)
                .Select(g => g.First());
        }

        public async Task<IEnumerable<Measurement>> GetForLast24h(long locationId)
        {
            var yesterdaySameTime = DateTime.UtcNow.AddDays(-1);

            return await _context.Measurement
                 .Include(m => m.SmogMeasurement).Include(m => m.WeatherMeasurement)
                .Where(m => m.LocationId == locationId && m.SmogMeasurement.PeriodTo > yesterdaySameTime)
                .ToListAsync();
        }

        public async Task<Measurement> GetLatest(long locationId)
        {
            var yesterdaySameTime = DateTime.UtcNow.AddDays(-1);

            return await _context.Measurement
               .Include(m => m.SmogMeasurement).Include(m => m.WeatherMeasurement)
               .Where(m => m.LocationId == locationId && m.SmogMeasurement.PeriodTo > yesterdaySameTime)
               .OrderByDescending(m => m.SmogMeasurement.PeriodTo)
               .FirstOrDefaultAsync();
        }
    }
}
