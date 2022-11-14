using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.Models.Context;

namespace Szpek.Data.Repositories
{
    public class AirQualityLevelRepository : IAirQualityLevelRepository
    {
        private readonly SzpekContext _context;

        public AirQualityLevelRepository(SzpekContext szpekContext) => _context = szpekContext;

        public async Task<IEnumerable<AirQualityLevel>> Get()
        {
            return await _context.AirQualityLevel.ToListAsync();
        }
    }
}
