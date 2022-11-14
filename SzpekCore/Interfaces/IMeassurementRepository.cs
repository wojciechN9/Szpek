using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Szpek.Core.Models;

namespace Szpek.Core.Interfaces
{
    public interface IMeassurementRepository
    {
        Task Create(Measurement meassurement);

        Task<IEnumerable<Measurement>> GetDescForSpecificDate(long locationId, DateTime date);

        Task<IEnumerable<Measurement>> GetLatests(IEnumerable<long> locationsIds);

        Task<IEnumerable<Measurement>> GetForLast24h(long locationId);

        Task<Measurement> GetLatest(long locationId);
    }
}
