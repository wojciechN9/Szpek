using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Szpek.Api.SensorApi;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Infrastructure.SensorContext;
using Xunit;

namespace Szpek.ApiUnitTests
{
    public class SensorApiV1ControllerTests
    {
        private readonly SensorApiV1Controller _controller;
        private readonly Mock<ISensorContext> _sensorContextMock;
        private readonly Mock<ILocationRepository> _locationRepositoryMocked;
        private readonly Mock<IAirQualityLevelRepository> _airQualityLevelRepositoryMocked;
        private readonly Mock<IMeassurementRepository> _meassurementRepositoryMocked;
        private readonly Mock<ISensorRepository> _sensorRepositoryMocked;
        private readonly Mock<IFirmwareRepository> _firmwareRepositoryMocked;
        private readonly Mock<ISensorLogRepository> _sensorLogRepositoryMocked;

        public SensorApiV1ControllerTests()
            : base()
        {
            _sensorContextMock = new Mock<ISensorContext>();
            _sensorContextMock.SetupGet(x => x.SensorId).Returns(1);
            _sensorContextMock.SetupGet(x => x.SensorCode).Returns("SENSOR_CODE");
            _locationRepositoryMocked = new Mock<ILocationRepository>();
            _airQualityLevelRepositoryMocked = new Mock<IAirQualityLevelRepository>();
            _meassurementRepositoryMocked = new Mock<IMeassurementRepository>();
            _sensorRepositoryMocked = new Mock<ISensorRepository>();
            _firmwareRepositoryMocked = new Mock<IFirmwareRepository>();
            _sensorLogRepositoryMocked = new Mock<ISensorLogRepository>();

            _controller = new SensorApiV1Controller(_sensorContextMock.Object, _locationRepositoryMocked.Object,
                _airQualityLevelRepositoryMocked.Object, _meassurementRepositoryMocked.Object, _sensorRepositoryMocked.Object,
                _firmwareRepositoryMocked.Object, _sensorLogRepositoryMocked.Object);
        }

        [Fact]
        public async Task ReportSmog()
        {
            _locationRepositoryMocked.Setup(l => l.GetActiveBySensorId(It.IsAny<long>())).ReturnsAsync(Location.Create(default, default));
            _airQualityLevelRepositoryMocked.Setup(a => a.Get()).ReturnsAsync(new List<AirQualityLevel>() { 
                new AirQualityLevel(default, PollutionType.PM10, default, default, default, default),
                new AirQualityLevel(default, PollutionType.PM25, default, default, default, default),
            });

            var result = await _controller.ReportSmog(new Application.SensorApi.v1.SensorApiV1ReportSmog()
            {
                Pm10Value = 12.3,
                Pm2_5Value = 5.3,
                Pm1Value = 11.22,
                SamplesCount = 1452,
                TimestampFrom = 1586023200,
                TimestampTo = 1586026800
            });

            result.Should().BeOfType<OkResult>();

            _meassurementRepositoryMocked.Verify(mr => mr.Create(It.Is<Measurement>(
                m => m.SmogMeasurement.Pm10Value == 12.3
                && m.SmogMeasurement.Pm25Value == 5.3
                && m.SmogMeasurement.Pm1Value == 11.22
                && m.SmogMeasurement.SamplesQuantity == 1452
                && m.SmogMeasurement.PeriodFrom == new DateTime(2020, 4, 4, 18, 0, 0, DateTimeKind.Utc)
                && m.SmogMeasurement.PeriodTo == new DateTime(2020, 4, 4, 19, 0, 0, DateTimeKind.Utc)
                && m.WeatherMeasurement == null))
            , Times.Once);
        }

        [Fact]
        public async Task ReportSmog_NoLocation()
        {
            _locationRepositoryMocked.Setup(l => l.GetActiveBySensorId(It.IsAny<long>())).ReturnsAsync(value: null);

            var result = await _controller.ReportSmog(new Application.SensorApi.v1.SensorApiV1ReportSmog());

            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be("ACTIVE_LOCATION_FOR_SENSOR_NOT_EXIST");
        }

        [Fact]
        public async Task ReportMeasurements()
        {
            _locationRepositoryMocked.Setup(l => l.GetActiveWithAddressBySensorId(It.IsAny<long>()))
                .ReturnsAsync(Location.Create(default, Address.Create(default, default, default, default, default, default, default, 100)));
            _airQualityLevelRepositoryMocked.Setup(a => a.Get()).ReturnsAsync(new List<AirQualityLevel>() {
                new AirQualityLevel(default, PollutionType.PM10, default, default, default, default),
                new AirQualityLevel(default, PollutionType.PM25, default, default, default, default),
            });

            var result = await _controller.ReportMeasurements(new Application.SensorApi.v1.SensorApiV1ReportMeasurements()
            {
                Pm10Value = 12.3,
                Pm2_5Value = 5.3,
                Pm1Value = 11.22,
                SamplesCount = 1452,
                TimestampFrom = 1586023200,
                TimestampTo = 1586026800,
                TemperatureCelsius = 24.12,
                PressureHPa = 1011, 
                HumidityPercent = 67
            });

            result.Should().BeOfType<OkResult>();

            _meassurementRepositoryMocked.Verify(mr => mr.Create(It.Is<Measurement>(
                m => m.SmogMeasurement.Pm10Value == 12.3
                && m.SmogMeasurement.Pm25Value == 5.3
                && m.SmogMeasurement.Pm1Value == 11.22
                && m.SmogMeasurement.SamplesQuantity == 1452
                && m.SmogMeasurement.PeriodFrom == new DateTime(2020, 4, 4, 18, 0, 0, DateTimeKind.Utc)
                && m.SmogMeasurement.PeriodTo == new DateTime(2020, 4, 4, 19, 0, 0, DateTimeKind.Utc)
                && m.WeatherMeasurement.CelciusTemperature == 24.12
                && m.WeatherMeasurement.AtmosphericPreassure != 0 ////exact test for pressure below
                && m.WeatherMeasurement.HumidityPercentage == 67
                && m.WeatherMeasurement.MeasurementDate == new DateTime(2020, 4, 4, 19, 0, 0, DateTimeKind.Utc)))
            , Times.Once);
        }

        [Fact]
        public async Task ReportMeasurements_AtmosfericPressureShouldBeCalculatedCorrectly()
        {
            var heightAboveSeaLevel = 100; //m

            _locationRepositoryMocked.Setup(l => l.GetActiveWithAddressBySensorId(It.IsAny<long>()))
                .ReturnsAsync(Location.Create(default, Address.Create(default, default, default, default, default, default, default, heightAboveSeaLevel)));
            _airQualityLevelRepositoryMocked.Setup(a => a.Get()).ReturnsAsync(new List<AirQualityLevel>() {
                new AirQualityLevel(default, PollutionType.PM10, default, default, default, default),
                new AirQualityLevel(default, PollutionType.PM25, default, default, default, default),
            });

            var result = await _controller.ReportMeasurements(new Application.SensorApi.v1.SensorApiV1ReportMeasurements()
            {
                PressureHPa = 1000,
                TemperatureCelsius = 20
            });

            result.Should().BeOfType<OkResult>();

            _meassurementRepositoryMocked.Verify(mr => mr.Create(It.Is<Measurement>(
                m => m.WeatherMeasurement.AtmosphericPreassure == 1011.7217994602751)), Times.Once);
        }

        [Fact]
        public async Task ReportMeasurements_NoLocation()
        {
            _locationRepositoryMocked.Setup(l => l.GetActiveBySensorId(It.IsAny<long>())).ReturnsAsync(value: null);

            var result = await _controller.ReportMeasurements(new Application.SensorApi.v1.SensorApiV1ReportMeasurements());

            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be("ACTIVE_LOCATION_FOR_SENSOR_NOT_EXIST");
        }

        [Fact]
        public async Task ReportStartup_KnownFirmware()
        {
            var currentFirmware = "firm1";
            var newFirmware = "firm2";

            _sensorRepositoryMocked.Setup(s => s.GetWithFirmware(It.IsAny<long>())).ReturnsAsync(
                new Sensor { InstalledFirmware = new Firmware(currentFirmware, new byte[] { 1, 2, 3, 4, 5 }, DateTime.UtcNow)});
            _firmwareRepositoryMocked.Setup(f => f.Get(It.Is<string>(name => name == newFirmware))).ReturnsAsync(
                new Firmware("firm2", new byte[] { 7, 8, 9, 10, 11 }, DateTime.UtcNow));

            var result = await _controller.ReportStartup(new Application.SensorApi.v1.SensorApiV1ReportStartup()
            {
                FirmwareName = newFirmware
            });

            result.Should().BeOfType<OkResult>();

            _sensorRepositoryMocked.Verify(s => s.Update(It.Is<Sensor>(se => se.InstalledFirmware.Name == newFirmware)));
        }

        [Fact]
        public async Task ReportStartup_UnknownFirmware()
        {
            var nonExistentFirmwareName = "unknown";

            _sensorRepositoryMocked.Setup(s => s.GetWithFirmware(It.IsAny<long>())).ReturnsAsync(
                new Sensor { InstalledFirmware = new Firmware("aaa", new byte[] { 1, 2, 3, 4, 5 }, DateTime.UtcNow) });
            _firmwareRepositoryMocked.Setup(f => f.Get(It.Is<string>(name => name == nonExistentFirmwareName))).ReturnsAsync(
               value: null);

            var result = await _controller.ReportStartup(new Application.SensorApi.v1.SensorApiV1ReportStartup()
            {
                FirmwareName = nonExistentFirmwareName
            });

            result.Should().BeOfType<OkResult>();
            _sensorRepositoryMocked.Verify(s => s.Update(It.Is<Sensor>(se => se.InstalledFirmware == null)));
        }

        [Fact]
        public async Task Log()
        {
            var logMessage = "TEST log";

            var result = await _controller.Log(new Application.SensorApi.v1.SensorApiV1Log()
            {
                Message = logMessage
            });

            result.Should().BeOfType<OkResult>();
            _sensorLogRepositoryMocked.Verify(sl => sl.Create(It.Is<SensorLog>(s => s.Message.Contains(logMessage))));
        }

        [Fact]
        public async Task Trace()
        {
            var invocations = new List<string>();
            _sensorLogRepositoryMocked
                .Setup(f => f.Create(It.IsAny<SensorLog>()))
                .Callback<SensorLog>(sl => invocations.Add(sl.Message));

            var result = await _controller.Trace(new Application.SensorApi.v1.SensorApiV1Trace()
            {
                Messages = new string[]
                {
                    "oldest",
                    "100",
                    "200",
                    "300",
                    "newest"
                }
            });

            result.Should().BeOfType<OkResult>();
            _sensorLogRepositoryMocked.Verify(sl => sl.Create(It.IsAny<SensorLog>()), Times.Exactly(5));

            invocations.Should().Equal(new[]
            {
                "oldest",
                "100",
                "200",
                "300",
                "newest"
            });
        }

        [Fact]
        public async Task GetRecommendedFirmware_NoFirmware()
        {
            _sensorRepositoryMocked.Setup(s => s.GetWithFirmware(It.IsAny<long>())).ReturnsAsync(
                 new Sensor { RecommendedFirmware = null });
            var result = await _controller.GetRecommendedFirmwareMetadata();

            result.Result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.Should().Be("NO_RECOMMENDED_FIRMWARE");
        }

        [Fact]
        public async Task GetRecommendedFirmware_Set()
        {
            var firmwareName = "firm1";
            var firmwareDateTime = DateTime.UtcNow;

            _sensorRepositoryMocked.Setup(s => s.GetWithFirmware(It.IsAny<long>())).ReturnsAsync(
              new Sensor { RecommendedFirmware = new Firmware(firmwareName, new byte[] { 1, 2, 3, 4, 5 }, firmwareDateTime) });

            var result = await _controller.GetRecommendedFirmwareMetadata();

            result.Value.Name.Should().Be(firmwareName);
            result.Value.ReleaseTimestamp.Should().Be(((DateTimeOffset)firmwareDateTime).ToUnixTimeSeconds());
        }

        [Fact]
        public async Task DownloadFirmware()
        {
            var firmwareName = "firm1";
            var firmwareContent = new byte[] { 1, 2, 3, 4, 5 };
            _firmwareRepositoryMocked.Setup(f => f.Get(It.IsAny<string>())).ReturnsAsync(
                new Firmware(firmwareName, firmwareContent, DateTime.UtcNow));

            var result = await _controller.DownloadFirmware(firmwareName);

            result.Should().BeOfType<FileContentResult>();
            var fcResult = (FileContentResult)result;
            fcResult.FileDownloadName.Should().Be(firmwareName);
            fcResult.FileContents.Should().BeEquivalentTo(firmwareContent, c => c.WithStrictOrdering());
        }

        [Fact]
        public async Task DownloadFirmware_NotFound()
        {
            _firmwareRepositoryMocked.Setup(f => f.Get(It.IsAny<string>())).ReturnsAsync(value: null);

            var result = await _controller.DownloadFirmware("unknown_firmware");

            result.Should().BeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result).Value.Should().Be("GIVEN_FIRMWARE_NOT_FOUND");
        }
    }
}
