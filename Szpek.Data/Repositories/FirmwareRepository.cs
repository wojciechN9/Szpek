using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.Models.Context;

namespace Szpek.Data.Repositories
{
    public class FirmwareRepository : IFirmwareRepository
    {
        private readonly SzpekContext _context;
        public FirmwareRepository(SzpekContext context) => _context = context;

        public async Task<Firmware> Get(string name)
        {
            return await _context.Firmware
                .SingleOrDefaultAsync(f => f.Name == name);
        }
    }
}
