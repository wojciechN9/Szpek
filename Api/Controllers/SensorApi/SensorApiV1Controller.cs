using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Szpek.Application.SensorApi.v1;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.SensorContext;

namespace Szpek.Api.SensorApi
{
    [Route("sensorApi/v1")]
    [ApiController]
    public class SensorApiV1Controller : ControllerBase
    {
        private readonly ISensorContext _sensorContext;
        private readonly ILocationRepository _locationRepository;
        private readonly IAirQualityLevelRepository _airQualityLevelRepository;
        private readonly IMeassurementRepository _meassurementRepository;
        private readonly ISensorRepository _sensorRepository;
        private readonly IFirmwareRepository _firmwareRepository;
        private readonly ISensorLogRepository _sensorLogRepository;

        public SensorApiV1Controller(
            ISensorContext sensorContext, 
            ILocationRepository locationRepository, 
            IAirQualityLevelRepository airQualityLevelRepository,
            IMeassurementRepository meassurementRepository,
            ISensorRepository sensorRepository,
            IFirmwareRepository firmwareRepository,
            ISensorLogRepository sensorLogRepository)
        {
            _sensorContext = sensorContext;
            _locationRepository = locationRepository;
            _airQualityLevelRepository = airQualityLevelRepository;
            _meassurementRepository = meassurementRepository;
            _sensorRepository = sensorRepository;
            _firmwareRepository = firmwareRepository;
            _sensorLogRepository = sensorLogRepository;
        }

        [HttpPost("smog")]
        public async Task<IActionResult> ReportSmog(SensorApiV1ReportSmog smogReport)
        {
            var location = await _locationRepository.GetActiveBySensorId(_sensorContext.SensorId);
            if (location == null)
            {
                return NotFound("ACTIVE_LOCATION_FOR_SENSOR_NOT_EXIST");
            }

            var airQualities = await _airQualityLevelRepository.Get();

            var smogMeasurement = SmogMeasurement.Create(
                smogReport.Pm10Value,
                smogReport.Pm2_5Value,
                smogReport.SamplesCount,
                DateTimeOffset.FromUnixTimeSeconds(smogReport.TimestampFrom).UtcDateTime,
                DateTimeOffset.FromUnixTimeSeconds(smogReport.TimestampTo).UtcDateTime);

            smogMeasurement.SetPm1Value(smogReport.Pm1Value);
            smogMeasurement.SetAirQualities(airQualities);

            var measurement = Measurement.Create(location.Id, smogMeasurement, null);

            await _meassurementRepository.Create(measurement);

            var sensorLog = SensorLog.Create(_sensorContext.SensorId, "API - Smog reported");
            await _sensorLogRepository.Create(sensorLog);

            return Ok();
        }

        [HttpPost("measurements")]
        public async Task<IActionResult> ReportMeasurements(SensorApiV1ReportMeasurements measurementsReport)
        {
            var location = await _locationRepository.GetActiveWithAddressBySensorId(_sensorContext.SensorId);
            if (location == null)
            {
                return NotFound("ACTIVE_LOCATION_FOR_SENSOR_NOT_EXIST");
            }

            var dateTimeFrom = DateTimeOffset.FromUnixTimeSeconds(measurementsReport.TimestampFrom).UtcDateTime;
            var dateTimeTo = DateTimeOffset.FromUnixTimeSeconds(measurementsReport.TimestampTo).UtcDateTime;

            var airQualities = await _airQualityLevelRepository.Get();
            var smogMeasurement = SmogMeasurement.Create(
                measurementsReport.Pm10Value,
                measurementsReport.Pm2_5Value,
                measurementsReport.SamplesCount,
                dateTimeFrom,
                dateTimeTo);
            smogMeasurement.SetPm1Value(measurementsReport.Pm1Value);
            smogMeasurement.SetAirQualities(airQualities);

            var weatherMeasurement = WeatherMeasurement.Create(
                measurementsReport.TemperatureCelsius,
                measurementsReport.PressureHPa,
                measurementsReport.HumidityPercent,
                dateTimeTo);
            weatherMeasurement.CalculateBarometricFormula(location.Address.Height);

            var measurement = Measurement.Create(location.Id, smogMeasurement, weatherMeasurement);
            await _meassurementRepository.Create(measurement);

            var sensorLog = SensorLog.Create(_sensorContext.SensorId, "API - Measurements (smog + weather) reported");
            await _sensorLogRepository.Create(sensorLog);

            return Ok();
        }

        [HttpPost("startup")]
        public async Task<IActionResult> ReportStartup(SensorApiV1ReportStartup startupReport)
        {
            var sensor = await _sensorRepository.GetWithFirmware(_sensorContext.SensorId);
            var firmwareStatusInfo = startupReport.FirmwareName == sensor.RecommendedFirmware?.Name ? "OK" : "NEEDS UPGRADE";

            var sensorLog = SensorLog.Create(
                _sensorContext.SensorId,
                 $"API - Sensor started with firmware {startupReport.FirmwareName}." +
                $" Recommended firmware is {sensor.RecommendedFirmware?.Name} - {firmwareStatusInfo}");

            await _sensorLogRepository.Create(sensorLog);

            if (sensor.InstalledFirmware?.Name != startupReport.FirmwareName)
            {
                sensor.InstalledFirmware = await _firmwareRepository.Get(startupReport.FirmwareName);
            }

            await _sensorRepository.Update(sensor);
            return Ok();
        }

        [HttpPost("log")]
        public async Task<IActionResult> Log(SensorApiV1Log log)
        {
            var sensorLog = SensorLog.Create(_sensorContext.SensorId, log.Message);
            await _sensorLogRepository.Create(sensorLog);

            return Ok();
        }

        [HttpPost("trace")]
        public async Task<IActionResult> Trace(SensorApiV1Trace trace)
        {
            foreach (var message in trace.Messages)
            {
                await _sensorLogRepository.Create(SensorLog.Create(_sensorContext.SensorId, message));
            }

            return Ok();
        }

        [HttpGet("firmware/recommended")]
        public async Task<ActionResult<SensorApiV1FirmwareMetadata>> GetRecommendedFirmwareMetadata()
        {
            var sensor = await _sensorRepository.GetWithFirmware(_sensorContext.SensorId);

            var sensorLog = SensorLog.Create(_sensorContext.SensorId, $"API - Asked for recommended firmware, it is {sensor.RecommendedFirmware?.Name}");
            await _sensorLogRepository.Create(sensorLog);

            if (sensor.RecommendedFirmware == null)
            {
                return NotFound("NO_RECOMMENDED_FIRMWARE");
            }

            return new SensorApiV1FirmwareMetadata()
            {
                Name = sensor.RecommendedFirmware.Name,
                ReleaseTimestamp = ((DateTimeOffset)sensor.RecommendedFirmware.ReleaseDateTimeUtc).ToUnixTimeSeconds()
            };
        }

        [HttpGet("firmware/download/{name}")]
        public async Task<IActionResult> DownloadFirmware(string name)
        {
            var firmware = await _firmwareRepository.Get(name);
            if (firmware == null)
            {
                return NotFound("GIVEN_FIRMWARE_NOT_FOUND");
            }

            var sensorLog = SensorLog.Create(_sensorContext.SensorId, $"API - Downloading firmware {name} started");
            await _sensorLogRepository.Create(sensorLog);

            return File(firmware.Content, "application/octet-stream", firmware.Name);
        }
    }
}
