using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Szpek.Api.Mappings;
using Szpek.Application.Sensor;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.Authorization;

namespace Szpek.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleNames.Admin)]
    public class SensorsController : ControllerBase
    {
        private readonly ISensorRepository _sensorRepository;
        private readonly ISensorOwnerRepository _sensorOwnerRepository;

        public SensorsController(ISensorRepository sensorRepository, ISensorOwnerRepository sensorOwnerRepository)
        {
            _sensorRepository = sensorRepository;
            _sensorOwnerRepository = sensorOwnerRepository;
        }

        [HttpPost]
        public async Task<ActionResult> Post(SensorCreate newSensor)
        {
            var sensorOwner = await _sensorOwnerRepository.Get(newSensor.OwnerId);
            var sensor = await _sensorRepository.GetByCodeOrPublicKey(newSensor.Code, newSensor.PublicKey);

            if (sensorOwner == null)
            {
                return UnprocessableEntity("SENSOROWNER_WITH_SPECIFIED_ID_NOT_EXIST");
            }

            if (sensor != null)
            {
                return UnprocessableEntity("CODE_OR_PUBLIC_KEY_EXIST");
            }

            var sensorToCreate = Sensor.Create(
                newSensor.Code, newSensor.OwnerId, newSensor.PublicKey, newSensor.IsPrivate);

            await _sensorRepository.Create(sensorToCreate);
            return Ok(sensorToCreate.Id);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SensorRead>>> Get()
        {
            var sensorData = await _sensorRepository.GetWithSensorOwnerAndLocations();

            return Ok(sensorData.ToSensorRead());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SensorDetailsRead>> Get(long id)
        {
            var sensor = await _sensorRepository.GetWithSensorOwnerLocationAndAddress(id);

            if (sensor == null)
            {
                return NotFound("SENSOR_WITH_SPECIFIED_ID_NOT_EXIST");
            }

            return sensor.ToSensorDetailsRead();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(long id, SensorUpdate sensorUpdate)
        {
            if (id != sensorUpdate.Id)
            {
                return BadRequest("ID_MISMATCH");
            }

            var sensor = await _sensorRepository.Get(sensorUpdate.Id);

            if (sensor == null)
            {
                return NotFound("SENSOR_WITH_SPECIFIED_ID_NOT_EXIST");
            }

            sensor.SetPrivacy(sensorUpdate.IsPrivate);
            await _sensorRepository.Update(sensor);

            return Ok();
        }
    }
}
