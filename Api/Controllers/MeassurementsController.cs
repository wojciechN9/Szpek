using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Szpek.Api.Mappings;
using Szpek.Application.Location;
using Szpek.Application.Meassurement;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.Authorization;

namespace Szpek.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeassurementsController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMeassurementRepository _meassurementRepository;
        private readonly IAirQualityLevelRepository _airQualityLevelRepository;
        private readonly IUserVerifier _userVerifier;

        public MeassurementsController(
            ILocationRepository locationRepository,
            IMeassurementRepository meassurementRepository,
            IAirQualityLevelRepository airQualityLevelRepository,
            IUserVerifier roleVerifier)
        {
            _locationRepository = locationRepository;
            _meassurementRepository = meassurementRepository;
            _airQualityLevelRepository = airQualityLevelRepository;
            _userVerifier = roleVerifier;
        }
        //add sensor data by admin - good for debuging
        [HttpPost]
        //[AllowAnonymous]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Post(MeassurementCreate newMeassurement)
        {
            var location = await _locationRepository.GetActiveBySensorCode(newMeassurement.SensorCode);

            if (location == null)
            {
                return UnprocessableEntity("ACTIVE_LOCATION_FOR_SPECIFIED_SENSOR_ID_NOT_EXIST");
            }

            var airQualities = await _airQualityLevelRepository.Get();

            var smogMeasurement = SmogMeasurement.Create(
                newMeassurement.Pm10Value,
                newMeassurement.Pm25Value,
                newMeassurement.SamplesQuantity,
                newMeassurement.PeriodFrom,
                newMeassurement.PeriodTo);

            smogMeasurement.SetPm1Value(newMeassurement.Pm1Value);
            smogMeasurement.SetAirQualities(airQualities);

            var measurement = Measurement.Create(location.Id, smogMeasurement, null);

            await _meassurementRepository.Create(measurement);
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<LocationMeassurementsRead>>> GetCurrentMeassurements()
        {
            var locations = await _locationRepository.GetActiveNonPrivate();
            var latestsMeassurements = await _meassurementRepository.GetLatests(locations.Select(l => l.Id));
            var locationMeassurementListRead = new List<LocationMeassurementsRead>();

            foreach (var location in locations)
            {
                var meassurement = latestsMeassurements.Where(m => m.LocationId == location.Id).FirstOrDefault();
                if (meassurement != null)
                {
                    locationMeassurementListRead.Add(
                        location.ToLocationMeassurementsRead(new List<Measurement>() { meassurement }));
                }
            }

            return locationMeassurementListRead;
        }

        [HttpGet("{locationId}")]
        [AllowAnonymous]
        public async Task<ActionResult<LocationMeassurementsRead>> GetLastDay(long locationId)
        {
            var location = await _locationRepository.GetActiveNonPrivateWithAddress(locationId);
            var meassurements = await _meassurementRepository.GetForLast24h(locationId);

            if (location == null)
            {
                return NotFound("LOCATION_NOT_EXIST_OR_IS_NOT_ACTIVE");
            }

            if (!meassurements.Any())
            {
                return NotFound("NO_MEASSUREMENTS_SINCE_LAST_24H");
            }

            return location.ToLocationMeassurementsRead(meassurements);
        }
             
        [HttpGet("{locationId}/{date}")]
        [Authorize(Roles = RoleNames.SensorOwner)]
        public async Task<ActionResult<IEnumerable<MeasurementRead>>> GetMeassurementsByDate(long locationId, DateTime date)
        {
            try
            {
                var userId = HttpContext.User.Identity.Name;
              
                if (!await _userVerifier.IsSpecifiedSensorOwnerOrAdmin(userId, locationId))
                {
                    return Unauthorized();
                }

                var meassurements = await _meassurementRepository.GetDescForSpecificDate(locationId, date);                    

                return meassurements.ToMeasurementsRead().ToList();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}