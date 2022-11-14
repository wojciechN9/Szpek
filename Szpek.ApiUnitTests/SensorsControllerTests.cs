using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Szpek.Api.Controllers;
using Szpek.Application.Sensor;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Xunit;

namespace Szpek.ApiUnitTests
{
    public class SensorsControllerTests
    {
        private readonly SensorsController _sensorsController;
        private readonly Mock<ISensorRepository> _sensorRepositoryMocked;
        private readonly Mock<ISensorOwnerRepository> _sensorOwnerRepositoryMocked;

        public SensorsControllerTests()
        {
            _sensorRepositoryMocked = new Mock<ISensorRepository>();
            _sensorOwnerRepositoryMocked = new Mock<ISensorOwnerRepository>();
            _sensorsController = new SensorsController(_sensorRepositoryMocked.Object, _sensorOwnerRepositoryMocked.Object);
        }

        [Fact]
        public async Task CreateSensorWithNotExistedOwner_ShouldUnprocessableEntityBeReturned()
        {
            _sensorOwnerRepositoryMocked.Setup(so => so.Get(It.IsAny<long>())).ReturnsAsync(value: null);
            var sensorDto = new SensorCreate() { OwnerId = default, Code = "Michow" };
            var actionResult = await _sensorsController.Post(sensorDto);

            Assert.IsType<UnprocessableEntityObjectResult>(actionResult);
            Assert.Equal("SENSOROWNER_WITH_SPECIFIED_ID_NOT_EXIST", (actionResult as UnprocessableEntityObjectResult).Value);
        }

        [Fact]
        public async Task CreateSensorWithSameCode_ShouldUnprocessableEntityBeReturned()
        {
            var existingSensorCode = "Michow";
            var sensorDto = new SensorCreate() { OwnerId = default, Code = existingSensorCode };

            _sensorOwnerRepositoryMocked.Setup(so => so.Get(It.IsAny<long>()))
                .ReturnsAsync(SensorOwner.Create(default, default, default));
            _sensorRepositoryMocked.Setup(s =>
                s.GetByCodeOrPublicKey(It.Is<string>(x => x == existingSensorCode), It.IsAny<string>()))
                .ReturnsAsync(Sensor.Create(existingSensorCode, default, default, default));

            var actionResult = await _sensorsController.Post(sensorDto);

            Assert.IsType<UnprocessableEntityObjectResult>(actionResult);
            Assert.Equal("CODE_OR_PUBLIC_KEY_EXIST", (actionResult as UnprocessableEntityObjectResult).Value);
        }

        [Fact]
        public async Task CreateSensor_ShouldBeOkResultReturnAndRepoCalled()
        {
            var sensorCode = "Michow";
            var sensorDto = new SensorCreate() { OwnerId = default, Code = sensorCode };

            _sensorOwnerRepositoryMocked.Setup(so => so.Get(It.IsAny<long>()))
             .ReturnsAsync(SensorOwner.Create(default, default, default));
            _sensorRepositoryMocked.Setup(s =>
                s.GetByCodeOrPublicKey(It.Is<string>(x => x == sensorCode), It.IsAny<string>()))
                .ReturnsAsync(value: null);

            var actionResult = await _sensorsController.Post(sensorDto);

            Assert.IsType<OkObjectResult>(actionResult);
            _sensorRepositoryMocked.Verify(s => s.Create(It.Is<Sensor>(se => se.Code == sensorCode)));
        }

        [Fact]
        public async Task GetSensorsWhenNoSensors_EmptyListShouldBeReturned()
        {
            var actionResult = await _sensorsController.Get();

            Assert.IsType<ActionResult<IEnumerable<SensorRead>>>(actionResult);
            Assert.Empty((actionResult.Result as OkObjectResult).Value as IEnumerable<SensorRead>);
        }

        [Fact]
        public async Task GetSensorsWhenSensorExists_ShouldBeReturned()
        {
            _sensorRepositoryMocked.Setup(s => s.GetWithSensorOwnerAndLocations())
                .ReturnsAsync(new List<Sensor>() { Sensor.Create(default, default, default, default) });

            var actionResult = await _sensorsController.Get();

            Assert.IsType<ActionResult<IEnumerable<SensorRead>>>(actionResult);
            Assert.Single((actionResult.Result as OkObjectResult).Value as IEnumerable<SensorRead>);
        }

        [Fact]
        public async Task GetSensorByIdWhenNoSensorExist_ShouldBeNotFound()
        {
            _sensorRepositoryMocked.Setup(s => s.GetWithSensorOwnerLocationAndAddress(It.IsAny<long>()))
                .ReturnsAsync(value: null);

            var actionResult = await _sensorsController.Get(default);

            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal("SENSOR_WITH_SPECIFIED_ID_NOT_EXIST", (actionResult.Result as NotFoundObjectResult).Value);
        }

        [Fact]
        public async Task GetSensorByIdWhenSensorExists_ShouldBeReturned()
        {
            var sensorId = 1L;
            var sensorCode = "Michow";

            _sensorRepositoryMocked.Setup(s => s.GetWithSensorOwnerLocationAndAddress(It.Is<long>(x => x == sensorId)))
                    .ReturnsAsync(new Sensor(sensorCode, new List<Location>()));

            var actionResult = await _sensorsController.Get(sensorId);

            Assert.Equal(sensorCode, actionResult.Value.Code);
        }

        [Fact]
        public async Task UpdateSensor_IdsAreDifferent_BadRequestShouldBeReturned()
        {
            var sensorId = 1;
            var sensorDifferentId = 2;
            var actionResult = await _sensorsController.Update(sensorId, new SensorUpdate() { Id = sensorDifferentId, IsPrivate = default });

            Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal("ID_MISMATCH", (actionResult as BadRequestObjectResult).Value);
        }

        [Fact]
        public async Task UpdateSensor_SensorWithGivenIdNotExist_NotFoundShouldBeReturned()
        {
            var sensorId = 1;
            _sensorRepositoryMocked.Setup(x => x.Get(It.Is<long>(id => id == sensorId)))
                .ReturnsAsync(value: null);

            var actionResult = await _sensorsController.Update(sensorId, new SensorUpdate() { Id = sensorId, IsPrivate = default });

            Assert.IsType<NotFoundObjectResult>(actionResult);
            Assert.Equal("SENSOR_WITH_SPECIFIED_ID_NOT_EXIST", (actionResult as NotFoundObjectResult).Value);
        }

        [Fact]
        public async Task UpdateSensor_SensorShouldBeUpdated()
        {
            var isPrivate = true;
            var sensorUpdate = new SensorUpdate() { Id = default, IsPrivate = isPrivate };

            _sensorRepositoryMocked.Setup(x => x.Get(It.IsAny<long>()))
                .ReturnsAsync(new Sensor());

            var actionResult = await _sensorsController.Update(default, sensorUpdate);

            _sensorRepositoryMocked.Verify(x => x.Update(It.Is<Sensor>(s => s.IsPrivate == isPrivate)));
            Assert.IsType<OkResult>(actionResult);
        }
    }
}
