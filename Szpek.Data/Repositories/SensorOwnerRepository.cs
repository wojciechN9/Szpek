using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.Models.Context;

namespace Szpek.Data.Repositories
{
    public class SensorOwnerRepository : ISensorOwnerRepository
    {
        private readonly SzpekContext _context;
        //user manager uses dbContext
        private readonly UserManager<User> _userManager;

        public SensorOwnerRepository(SzpekContext szpekContext, UserManager<User> userManager) 
        { 
            _context = szpekContext;
            _userManager = userManager;
        }

        public async Task Create(SensorOwner sensorOwner)
        {
            await _context.AddAsync(sensorOwner);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SensorOwner>> GetWithSensors()
        {
            return await this._context.SensorOwner
                .Include(s => s.Sensors)
                .ToListAsync();
        }

        public async Task<SensorOwner> Get(long id)
        {
            return await _context.SensorOwner
                .SingleOrDefaultAsync(so => so.Id == id);
        }

        public async Task<SensorOwner> GetWithSensors(long id)
        {
            return await this._context.SensorOwner
                .Include(s => s.Sensors)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SensorOwner> GetWithSensorsAndLocations(long id)
        {
            return await _context.SensorOwner
                .Include(so => so.Sensors).ThenInclude(s => s.Locations)
                .SingleOrDefaultAsync(so => so.Id == id);
        }

        public async Task<long> GetIdFromHttpContextUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var claims = await _userManager.GetClaimsAsync(user);
            var sensorOwnerIdClaim = claims.SingleOrDefault(c => c.Type == "SensorOwnerId");

            if (sensorOwnerIdClaim == null)
            {
                throw new ArgumentException("SENSOROWNER_FOR_THIS_USERNAME_NOT_EXIST");
            }

            return long.Parse(sensorOwnerIdClaim.Value);
        }
    }
}
