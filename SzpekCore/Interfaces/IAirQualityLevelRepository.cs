using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Szpek.Core.Models;

namespace Szpek.Core.Interfaces
{
    public interface IAirQualityLevelRepository
    {
        Task<IEnumerable<AirQualityLevel>> Get();
    }
}
