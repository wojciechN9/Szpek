using System.Threading.Tasks;

namespace Szpek.Infrastructure.Authorization
{
    public interface IUserVerifier
    {
        Task<bool> IsSpecifiedSensorOwnerOrAdmin(string userId, long locationId);
    }
}
