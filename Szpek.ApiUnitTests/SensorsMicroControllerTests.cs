using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Szpek.Api.Controllers;
using Szpek.Application.Meassurement;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Xunit;

namespace Szpek.ApiUnitTests
{

    public class SensorsMicroControllerTests

    {
        //sensor = {
        //  "pm10Value": 10.1,
        //  "pm25Value": 8,
        //  "pm1Value": 5,
        //  "samplesQuantity": 200,
        //  "periodFrom": "2019-09-02T11:44:30.321",
        //  "periodTo": "2019-09-02T12:44:30.322",
        //  "sensorCode": "michow"
        // }
        private readonly SensorsMicroController _sensorsMicroController;
        private readonly Mock<ILocationRepository> _locationRepositoryMocked;
        private readonly Mock<ISensorRepository> _sensorRepositoryMocked;
        private readonly Mock<IMeassurementRepository> _meassurementRepositoryMocked;
        private readonly Mock<IAirQualityLevelRepository> _airQualityLevelRepositoryMocked;
        private readonly Mock<ISensorLogRepository> _sensorLogRepositoryMocked;


        public SensorsMicroControllerTests()
        {
            _locationRepositoryMocked = new Mock<ILocationRepository>();
            _sensorRepositoryMocked = new Mock<ISensorRepository>();
            _meassurementRepositoryMocked = new Mock<IMeassurementRepository>();
            _airQualityLevelRepositoryMocked = new Mock<IAirQualityLevelRepository>();
            _sensorLogRepositoryMocked = new Mock<ISensorLogRepository>();
            _sensorsMicroController = new SensorsMicroController(
                _locationRepositoryMocked.Object, _sensorRepositoryMocked.Object,
                _meassurementRepositoryMocked.Object, _airQualityLevelRepositoryMocked.Object,
                _sensorLogRepositoryMocked.Object);
        }

        [Fact]
        public async Task PostMicrocontrollerData_WhenRequestHasIncorrectData_UnprocessableEntityShouldBeReturned()
        {
            var request = new SensorMeassurementDataModel()
            {
                Payload = "aaa",
                Signature = "bbbb"
            };

            var result = await _sensorsMicroController.Post(request);
            Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal("INCORRECT_REQUEST_FORMAT", (result as UnprocessableEntityObjectResult).Value);
        }

        [Fact]
        public async Task PostMicrocontrollerData_WhenRequestHasNoExistingSensor_UnprocessableEntityShouldBeReturned()
        {
            var request = new SensorMeassurementDataModel()
            {
                Payload = "eydwbTEwVmFsdWUnOiAxMC4xLCAncG0yNVZhbHVlJzogOCwgJ3NhbXBsZXNRdWFudGl0eSc6IDIwMCwgJ3BlcmlvZEZyb20nOiAnMjAxOS0wNy0wOVQxMDo0NDozMC4zMjEnLCAncGVyaW9kVG8nOiAnMjAxOS0wNy0wOVQxMDo0NDozMC4zMjInLCAnc2Vuc29yQ29kZSc6ICdtaWNob3cnfQ==",
                Signature = "brcNh2k79MA+lFGlajWD/RBDUfuUoK2v4Eh53/VB8pCUTu9nfL6oNAOEVm+7Jlfo4D7N6Yqk7IqghJ1QC8eCLgkljNGRoRVDUbcsolMsrrxheP+PU5NYVCCn5YfYu/mnqKFuIrlwTKo/QgPlHN+pEUnYS7LekiubsBOrFpR55BI="
            };

            _sensorRepositoryMocked.Setup(s => s.Get(It.IsAny<string>()))
              .ReturnsAsync(value: null);

            var result = await _sensorsMicroController.Post(request);
            Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal("SENSOR_WITH_SPECIFIED_ID_NOT_EXIST", (result as UnprocessableEntityObjectResult).Value);
        }

        [Fact]
        public async Task PostMicrocontrollerData_WhenKeyIsIncorrect_UnprocessableEntityShouldBeReturned()
        {
            var request = new SensorMeassurementDataModel()
            {
                Payload = "eydwbTEwVmFsdWUnOiAxMC4xLCAncG0yNVZhbHVlJzogOCwgJ3NhbXBsZXNRdWFudGl0eSc6IDIwMCwgJ3BlcmlvZEZyb20nOiAnMjAxOS0wNy0wOVQxMDo0NDozMC4zMjEnLCAncGVyaW9kVG8nOiAnMjAxOS0wNy0wOVQxMDo0NDozMC4zMjInLCAnc2Vuc29yQ29kZSc6ICdtaWNob3cnfQ==",
                Signature = "brcNh2k79MA+lFGlajWD/RBDUfuUoK2v4Eh53/VB8pCUTu9nfL6oNAOEVm+7Jlfo4D7N6Yqk7IqghJ1QC8eCLgkljNGRoRVDUbcsolMsrrxheP+PU5NYVCCn5YfYu/mnqKFuIrlwTKo/QgPlHN+pEUnYS7LekiubsBOrFpR55BI="
            };
            //incorrect key
            var sensorPubKey = "ZHVwYQ==";

            _sensorRepositoryMocked.Setup(s => s.Get(It.IsAny<string>()))
                .ReturnsAsync(new Sensor(sensorPubKey));

            var result = await _sensorsMicroController.Post(request);
            Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal("INCORRECT_AUTHENTICATION", (result as UnprocessableEntityObjectResult).Value);
        }

        [Fact]
        public async Task PostMicrocontrollerData_WhenNoActiveLocation_UnprocessableEntityShouldBeReturned()
        {
            var request = new SensorMeassurementDataModel()
            {
                Payload = "eydwbTEwVmFsdWUnOiAxMC4xLCAncG0yNVZhbHVlJzogOCwgJ3BtMVZhbHVlJzogNSwgJ3NhbXBsZXNRdWFudGl0eSc6IDIwMCwgJ3BlcmlvZEZyb20nOiAnMjAxOS0wOS0wMlQxMTo0NDozMC4zMjEnLCAncGVyaW9kVG8nOiAnMjAxOS0wOS0wMlQxMjo0NDozMC4zMjInLCAnc2Vuc29yQ29kZSc6ICdtaWNob3cnfQ==",
                Signature = "qSfUxtRa1zmXm+nvKZQEWir1hLC6buuvGUMJLPmYh1U="
            };

            var sensorPubKey = "YjNEUUVCQVFVQUE0R05BRENCaVFLQmdRRERsbDdDVjc=";

            _sensorRepositoryMocked.Setup(s => s.Get(It.IsAny<string>()))
                .ReturnsAsync(new Sensor(sensorPubKey));
            _locationRepositoryMocked.Setup(s => s.Get(It.IsAny<long>()))
                .ReturnsAsync(value: null);

            var result = await _sensorsMicroController.Post(request);
            Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal("ACTIVE_LOCATION_FOR_SENSOR_NOT_EXIST", (result as UnprocessableEntityObjectResult).Value);
        }

        [Fact]
        public async Task PostMicrocontrollerData_WhenPublicKeyMaches_DataShouldBePosted()
        {
            var request = new SensorMeassurementDataModel()
            {
                Payload = "eydwbTEwVmFsdWUnOiAxMC4xLCAncG0yNVZhbHVlJzogOCwgJ3BtMVZhbHVlJzogNSwgJ3NhbXBsZXNRdWFudGl0eSc6IDIwMCwgJ3BlcmlvZEZyb20nOiAnMjAxOS0wOS0wMlQxMTo0NDozMC4zMjEnLCAncGVyaW9kVG8nOiAnMjAxOS0wOS0wMlQxMjo0NDozMC4zMjInLCAnc2Vuc29yQ29kZSc6ICdtaWNob3cnfQ==",
                Signature = "qSfUxtRa1zmXm+nvKZQEWir1hLC6buuvGUMJLPmYh1U="
            };

            var sensorCode = "michow";
            var sensorPubKey = "YjNEUUVCQVFVQUE0R05BRENCaVFLQmdRRERsbDdDVjc=";

            _sensorRepositoryMocked.Setup(s => s.Get(It.Is<string>(c => c == sensorCode)))
                .ReturnsAsync(new Sensor(sensorPubKey));
            _locationRepositoryMocked.Setup(l => l.GetActiveBySensorId(It.IsAny<long>()))
                .ReturnsAsync(Location.Create(default, default));
            _airQualityLevelRepositoryMocked.Setup(a => a.Get()).ReturnsAsync(new List<AirQualityLevel>() {
                new AirQualityLevel(default, PollutionType.PM10, default, default, default, default),
                new AirQualityLevel(default, PollutionType.PM25, default, default, default, default),
            });

            var result = await _sensorsMicroController.Post(request);

            Assert.IsType<OkResult>(result);
            _meassurementRepositoryMocked.Verify(m => m.Create(It.Is<Measurement>(me => me.SmogMeasurement.Pm10Value == 10.1)));
        }
    }
}
