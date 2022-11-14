using System.Threading.Tasks;
using Szpek.Core.Models;

namespace Szpek.Core.Interfaces
{
    public interface ISensorLogRepository
    {
        Task Create(SensorLog sensorLog);
    }
}
