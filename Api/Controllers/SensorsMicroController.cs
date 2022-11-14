using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Szpek.Application.Meassurement;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;

namespace Szpek.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsMicroController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ISensorRepository _sensorRepository;
        private readonly IMeassurementRepository _meassurementRepository;
        private readonly IAirQualityLevelRepository _airQualityLevelRepository;
        private readonly ISensorLogRepository _sensorLogRepository;

        public SensorsMicroController(
            ILocationRepository locationRepository,
            ISensorRepository sensorRepository,
            IMeassurementRepository meassurementRepository,
            IAirQualityLevelRepository airQualityLevelRepository,
            ISensorLogRepository sensorLogRepository)
        {
            _locationRepository = locationRepository;
            _sensorRepository = sensorRepository;
            _meassurementRepository = meassurementRepository;
            _airQualityLevelRepository = airQualityLevelRepository;
            _sensorLogRepository = sensorLogRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post(SensorMeassurementDataModel sensorData)
        {
            MeassurementCreate newMeassurement;
            try
            {
                newMeassurement = Deserialize(sensorData.Payload);
            }
            catch (FormatException)
            {
                return UnprocessableEntity("INCORRECT_REQUEST_FORMAT");
            }

            var sensor = await _sensorRepository.Get(newMeassurement.SensorCode);

            if (sensor == null)
            {
                return UnprocessableEntity("SENSOR_WITH_SPECIFIED_ID_NOT_EXIST");
            }

            try
            {
                Authenticate(sensorData, sensor.PublicKey);
            }
            catch (FormatException)
            {
                return UnprocessableEntity("INCORRECT_AUTHENTICATION");
            }

            var location = await _locationRepository.GetActiveBySensorId(sensor.Id);

            if (location == null)
            {
                return UnprocessableEntity("ACTIVE_LOCATION_FOR_SENSOR_NOT_EXIST");
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
            
            var sensorLog = SensorLog.Create(sensor.Id, "API_old - Smog reported");
            await _sensorLogRepository.Create(sensorLog);

            return Ok();
        }

        private MeassurementCreate Deserialize(string serializedData)
        {
            return JsonConvert.DeserializeObject<MeassurementCreate>(Encoding.ASCII.GetString(Convert.FromBase64String(serializedData)));
        }

        private void Authenticate(SensorMeassurementDataModel sensorData, string sensorKey)
        {
            //change when create sensor - or not?
            byte[] keyBytes = Convert.FromBase64String(sensorKey);

            if (keyBytes.Length != 32)
            {
                throw new FormatException("key should have 32bytes");
            }

            // Compute Payload hash
            byte[] computedHash;
            using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
            {
                computedHash = hmac.ComputeHash(Encoding.ASCII.GetBytes(sensorData.Payload)); // compute HMAC from base64 encoded payload
            }

            var signatureHash = Convert.FromBase64String(sensorData.Signature);

            if (computedHash.Length != signatureHash.Length)
            {
                throw new FormatException("lenght of hashes is not equal");
            }
            for (int i = 0; i < signatureHash.Length; i++)
            {
                if (computedHash[i] != signatureHash[i]) throw new FormatException("hashes are not the same");
            }
        }
    }
}