using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Szpek.Api.Mappings;
using Szpek.Application.SensorOwner;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.Authorization;

namespace Szpek.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsOwnersController : ControllerBase
    {
        private readonly ISensorOwnerRepository _sensorOwnerRepository;
        private readonly UserManager<User> _userManager;

        public SensorsOwnersController(
            ISensorOwnerRepository sensorOwnerRepository,
            UserManager<User> userManager)
        {
            _sensorOwnerRepository = sensorOwnerRepository;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<long>> Post(SensorOwnerCreate sensorOwnerCreate)
        {
            var user = await _userManager.FindByIdAsync(sensorOwnerCreate.UserId);

            if (user == null)
            {
                return UnprocessableEntity("USER_WITH_SPECIFIED_ID_NOT_EXIST");
            }
            if (user.SensorOwnerId != null)
            {
                return UnprocessableEntity("SENSOR_OWNER_FOR_THIS_USER_EXIST");
            }

            var sensorOwner = SensorOwner.Create(
                sensorOwnerCreate.Name, 
                sensorOwnerCreate.IsCompany,
                sensorOwnerCreate.Address);

            await _sensorOwnerRepository.Create(sensorOwner);

            user.AddSensorOwner(sensorOwner);
            await _userManager.AddClaimAsync(user, new Claim("SensorOwnerId", user.SensorOwnerId.ToString()));

            return Ok(sensorOwner.Id);
        }

        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<IEnumerable<SensorOwnerRead>>> Get()
        {
            var sensorOwners = await _sensorOwnerRepository.GetWithSensors();

            return Ok(sensorOwners.ToSensorsOwnersRead());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<ActionResult<SensorOwnerRead>> Get(long id)
        {
            var sensorOwner = await _sensorOwnerRepository.GetWithSensors(id);

            if (sensorOwner == null)
            {
                return UnprocessableEntity("SENSOR_OWNER_WITH_SPECIFIED_ID_NOT_EXIST");
            }

            return Ok(sensorOwner.ToSensorOwnerRead());
        }

        [HttpGet("my")]
        [Authorize(Roles = RoleNames.SensorOwner)]
        public async Task<ActionResult<IEnumerable<SensorOwnerRead>>> GetSensorOwnerInfo()
        {
            try
            {
                var sensorOwnerId = await _sensorOwnerRepository.GetIdFromHttpContextUser(HttpContext.User.Identity.Name);
                var sensorOwner = await _sensorOwnerRepository.GetWithSensorsAndLocations(sensorOwnerId);

                return Ok(sensorOwner.ToSensorOwnerRead());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }            
        }
    }
}