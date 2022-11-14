using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;

namespace Szpek.Infrastructure.Authorization
{
    public class UserVerifier : IUserVerifier
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ISensorOwnerRepository _sensorOwnerRepository;
        private readonly UserManager<User> _userManager;

        public UserVerifier(
            ILocationRepository locationRepository,
            ISensorOwnerRepository sensorOwnerRepository,
            UserManager<User> userManager)
        {
            _locationRepository = locationRepository;
            _sensorOwnerRepository = sensorOwnerRepository;
            _userManager = userManager;
        }

        public async Task<bool> IsSpecifiedSensorOwnerOrAdmin(string userId, long locationId)
        {
            var sensorOwnerId = await _sensorOwnerRepository.GetIdFromHttpContextUser(userId);

            if (!await _locationRepository.DoesBelongToSensorOwner(locationId, sensorOwnerId)
                && !await IsAdmin(userId))
            {
                return false;
            }

            return true;
        }

        public async Task<bool> IsAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.IsInRoleAsync(user, RoleNames.Admin);
        }
    }
}