using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.Models.Context;

namespace Szpek.Data.Repositories
{
    public class SensorLogRepository : ISensorLogRepository
    {
        private readonly SzpekContext _szpekContext;

        public SensorLogRepository(SzpekContext szpekContext) => _szpekContext = szpekContext;

        public async Task Create(SensorLog sensorLog)
        {
            await _szpekContext.AddAsync(sensorLog);
            await _szpekContext.SaveChangesAsync();
        }
    }
}
