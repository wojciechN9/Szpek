using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Szpek.Api.Mappings;
using Szpek.Application.Location;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.Authorization;

namespace Szpek.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ISensorRepository _sensorRepository;
        private readonly IUserVerifier _userVerifier;

        public LocationsController(
            ILocationRepository locationRepository,
            ISensorRepository sensorRepository,
            IUserVerifier roleVerifier)
        {
            _locationRepository = locationRepository;
            _sensorRepository = sensorRepository;
            _userVerifier = roleVerifier;
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult> NewSensorLocation(LocationCreate locationCreate)
        {
            var sensor = await _sensorRepository.Get(locationCreate.SensorId);

            if (sensor == null)
            {
                return UnprocessableEntity("SENSOR_WITH_SPECIFIED_ID_NOT_EXIST");
            }

            var currentLocation = await _locationRepository.GetActiveBySensorId(locationCreate.SensorId);

            if (currentLocation != null)
            {
                currentLocation.Deactivate();
            }

            var address = Address.Create(locationCreate.Address.City,
                    locationCreate.Address.Street,
                    locationCreate.Address.PostCode,
                    locationCreate.Address.Voivodeship,
                    locationCreate.Address.CountryCode,
                    locationCreate.Address.Latitude,
                    locationCreate.Address.Longitude,
                    locationCreate.Address.Height);

            var newLocation = Location.Create(locationCreate.SensorId, address);
            await _locationRepository.Create(newLocation);

            return Ok(newLocation.Id);
        }

        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<IEnumerable<LocationRead>>> Get()
        {
            var locations = await _locationRepository.GetWithAdresses();

            return Ok(locations.ToLocationsRead());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleNames.SensorOwner)]
        public async Task<ActionResult<LocationDetailsRead>> Get(long id)
        {
            try
            {
                var userId = HttpContext.User.Identity.Name;
                if (! await _userVerifier.IsSpecifiedSensorOwnerOrAdmin(userId, id))
                {
                    return Unauthorized();
                }                

                var location = await _locationRepository.GetLocationWithAddressAndSensor(id);

                if (location == null)
                {
                    return NotFound("LOCATION_WITH_SPECIFIED_ID_NOT_EXIST");
                }

                return Ok(location.ToLocationDetailsRead());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("enableOldLocation/{{locationId}}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> EnableOldLocation(long locationId, [Required]long sensorId)
        {
            var sensor = await _sensorRepository.Get(sensorId);
            var oldLocation = await _locationRepository.GetInactive(locationId);

            if (sensor == null || oldLocation == null)
            {
                return NotFound("SENSOR_OR_LOCATION_WITH_SPECIFIED_ID_NOT_EXIST");
            }

            var currentLocation = await _locationRepository.GetActiveBySensorId(sensorId);

            if (currentLocation != null)
            {
                currentLocation.Deactivate();
            }

            oldLocation.Activate(sensorId);
            await _locationRepository.Update(oldLocation);
            return NoContent();
        }

        [HttpPut]
        [Route("changeStreetAddress/{{locationId}}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> ChangeStreetAddress(long locationId, [Required]string streetAddress)
        {
            var location = await _locationRepository.Get(locationId);

            if (location == null)
            {
                return NotFound("LOCATION_WITH_SPECIFIED_ID_NOT_EXIST");
            }

            location.ChangeStreetAddress(streetAddress);
            await _locationRepository.Update(location);

            return NoContent();
        }
    }
}