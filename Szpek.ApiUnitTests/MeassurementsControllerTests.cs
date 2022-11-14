//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Szpek.Api.Controllers;
//using Szpek.Application.Location;
//using Szpek.Application.Meassurement;
//using Szpek.IntegrationTests.Settings;
//using Xunit;

//namespace Szpek.ApiUnitTests
//{
//    public class MeassurementsControllerTests : TestsBase
//    {
//        public MeassurementsControllerTests() : base() { }

//        [Fact]
//        public async Task PostMeassurementsWhenNoSensorActiveLocationExist_ShouldBeUnprocessableEntityThrown()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var meassurementsController = new MeassurementsController(context);
//                var meassurementDto = new MeassurementCreate() { SensorCode = default, };

//                var actionResult = await meassurementsController.Post(meassurementDto);

//                Assert.IsType<UnprocessableEntityObjectResult>(actionResult);
//            }
//        }

//        [Fact]
//        public async Task PostMeassurements_ShouldBeSaved()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationId = TestsUtils.CreateSampleLocationWithSensor(context, "Michow", "a");
//                var location = await context.Location.SingleAsync();
//                var meassurementsController = new MeassurementsController(context);
//                var meassurementDto = new MeassurementCreate()
//                {
//                    SensorCode = location.Sensor.Code,
//                    Pm10Value = default,
//                    Pm25Value = default,
//                    PeriodFrom = default,
//                    PeriodTo = default,
//                    SamplesQuantity = default
//                };

//                var actionResult = await meassurementsController.Post(meassurementDto);

//                Assert.IsType<OkResult>(actionResult);
//            }
//        }

//        [Fact]
//        public async Task PostMeassurements_ShouldBePm10QualityCalculatedCorrectly()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationId = TestsUtils.CreateSampleLocationWithSensor(context, "Michow", "a");
//                var location = await context.Location.SingleAsync();
//                var meassurementsController = new MeassurementsController(context);
//                var meassurementDto = new MeassurementCreate()
//                {
//                    SensorCode = location.Sensor.Code,
//                    Pm10Value = 22,
//                    PeriodTo = DateTime.Now,
//                    Pm25Value = default,
//                    PeriodFrom = default,
//                    SamplesQuantity = default
//                };

//                await meassurementsController.Post(meassurementDto);

//                var result = meassurementsController.GetLastDay(location.Id);

//                var meassurement = result.Result.Value.Meassurements.First();

//                Assert.Equal(AirQuality.Good, meassurement.Pm10Quality);
//            }
//        }

//        [Fact]
//        public async Task PostMeassurements_ShouldBePm25QualityCalculatedCorrectly()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationId = TestsUtils.CreateSampleLocationWithSensor(context, "Michow", "a");
//                var location = await context.Location.SingleAsync();
//                var meassurementsController = new MeassurementsController(context);
//                var meassurementDto = new MeassurementCreate()
//                {
//                    SensorCode = location.Sensor.Code,
//                    Pm25Value = 0,
//                    PeriodTo = DateTime.Now,
//                    Pm10Value = default,
//                    PeriodFrom = default,
//                    SamplesQuantity = default
//                };

//                await meassurementsController.Post(meassurementDto);

//                var result = meassurementsController.GetLastDay(location.Id);

//                var meassurement = result.Result.Value.Meassurements.First();

//                Assert.Equal(AirQuality.VeryGood, meassurement.Pm25Quality);
//            }
//        }

//        [Fact]
//        public async Task PostMeassurements_ShouldBeAirQualityCalculatedCorrectly()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationId = TestsUtils.CreateSampleLocationWithSensor(context, "Michow", "a");
//                var location = await context.Location.SingleAsync();
//                var meassurementsController = new MeassurementsController(context);
//                var meassurementDto = new MeassurementCreate()
//                {
//                    SensorCode = location.Sensor.Code,
//                    Pm10Value = 22,
//                    Pm25Value = 0,
//                    PeriodTo = DateTime.Now,
//                    PeriodFrom = default,
//                    SamplesQuantity = default
//                };

//                await meassurementsController.Post(meassurementDto);

//                var result = meassurementsController.GetLastDay(location.Id);

//                var meassurement = result.Result.Value.Meassurements.First();

//                Assert.Equal(AirQuality.Good, meassurement.AirQuality);
//            }
//        }

//        [Fact]
//        public async Task PostMeassurements_WhenValueIsMoreThan3000_ShouldBeQualityCalculatedAsError()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationId = TestsUtils.CreateSampleLocationWithSensor(context, "Michow", "a");
//                var location = await context.Location.SingleAsync();
//                var meassurementsController = new MeassurementsController(context);
//                var meassurementDto = new MeassurementCreate()
//                {
//                    SensorCode = location.Sensor.Code,
//                    Pm10Value = 5555,
//                    Pm25Value = default,
//                    PeriodTo = DateTime.Now,
//                    PeriodFrom = default,
//                    SamplesQuantity = default
//                };

//                await meassurementsController.Post(meassurementDto);

//                var result = meassurementsController.GetLastDay(location.Id);

//                var meassurement = result.Result.Value.Meassurements.First();

//                Assert.Equal(AirQuality.Error, meassurement.Pm10Quality);
//            }
//        }

//        [Fact]
//        public async Task GetCurrentMeassurements_ShouldBeGotten()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationId = TestsUtils.CreateSampleLocationWithSensor(context, "Michow", "a");
//                var location = await context.Location.SingleAsync();
//                var meassurementsController = new MeassurementsController(context);
//                var meassurementDto = new MeassurementCreate()
//                {
//                    SensorCode = location.Sensor.Code,
//                    Pm10Value = default,
//                    Pm25Value = default,
//                    PeriodFrom = default,
//                    PeriodTo = default,
//                    SamplesQuantity = default
//                };

//                await meassurementsController.Post(meassurementDto);

//                var actionResult = await meassurementsController.GetCurrentMeassurements();

//                Assert.IsType<ActionResult<IEnumerable<LocationMeassurementsRead>>>(actionResult);
//            }
//        }

//        [Fact]
//        public async Task GetCurrentMeassurementsWhenSensorIsPrivate_ShouldBeEmptyListReturned()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationId = TestsUtils.CreateSampleLocationWithSensor(
//                    context,
//                    "Michow",
//                    "a",
//                    isPrivate: true);
//                var location = await context.Location.SingleAsync();
//                var meassurementsController = new MeassurementsController(context);
//                var meassurementDto = new MeassurementCreate()
//                {
//                    SensorCode = location.Sensor.Code,
//                    Pm10Value = 10,
//                    Pm25Value = 10,
//                    PeriodFrom = DateTime.UtcNow.AddHours(-1),
//                    PeriodTo = DateTime.UtcNow,
//                    SamplesQuantity = 1000
//                };

//                await meassurementsController.Post(meassurementDto);

//                var actionResult = await meassurementsController.GetCurrentMeassurements();

//                Assert.Empty(actionResult.Value);
//            }
//        }

//        [Fact]
//        public async Task GetLastDayWhenLocationNotActive_NotFoundShouldBeReturned()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var meassurementsController = new MeassurementsController(context);
//                var locationsController = new LocationsController(context);
//                await TestsUtils.CreateSampleLocationWithSensor(context, sensorCode: "Michów", sensorKey: "a");
//                var oldLocation = await context.Location.FirstAsync();
//                var newlocationDto = new LocationCreate()
//                {
//                    SensorId = oldLocation.SensorId,
//                    Address = TestsUtils.GetSampleDtoAddress()
//                };
//                await locationsController.NewSensorLocation(newlocationDto);

//                var actionResult = await meassurementsController.GetLastDay(oldLocation.Id);

//                Assert.IsType<NotFoundObjectResult>(actionResult.Result);
//            }
//        }

//        [Fact]
//        public async Task GetLastDayWhenNoMeassurementFromLast24h_NotFoundShouldBeReturned()
//        {
//            double pm10Value = 0.5;
//            double pm25Value = 0.5;
//            DateTime periodTo = DateTime.Now.AddDays(-2);

//            using (var context = GetNewSzpekContext())
//            {
//                var meassurementsController = new MeassurementsController(context);
//                var locationId = await TestsUtils.CreateSampleLocationWithSensor(context,"Michow","a");
//                var location = await context.Location.SingleAsync();
//                await CreateMeassurement(meassurementsController, location.Sensor.Code, pm10Value, pm25Value, periodFrom: DateTime.Now.AddHours(-1), periodTo, samplesQuantity: 1);

//                var actionResult = await meassurementsController.GetLastDay(locationId);

//                Assert.IsType<NotFoundObjectResult>(actionResult.Result);
//            }
//        }

//        [Fact]
//        public async Task GetLastDayWhenSensorIsPrivate_NotFoundShouldBeReturned()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var meassurementsController = new MeassurementsController(context);
//                var locationId = await TestsUtils.CreateSampleLocationWithSensor(
//                    context,
//                    "Michow", 
//                    "a",
//                    isPrivate: true);
//                var location = await context.Location.SingleAsync();
//                await CreateMeassurement(meassurementsController, location.Sensor.Code, 0.5, 0.5, DateTime.Now.AddHours(-1), DateTime.UtcNow, 1);

//                var actionResult = await meassurementsController.GetLastDay(locationId);

//                Assert.IsType<NotFoundObjectResult>(actionResult.Result);
//            }
//        }

//        [Fact]
//        public async Task GetLastDay_LocationWithAddressAndMeassurementsShouldBeGotten()
//        {
//            double pm10Value = 0.5;
//            double pm25Value = 0.5;
//            DateTime periodTo = DateTime.Now;
//            var city = "Lublin";
//            var street = "Składowska";
//            var postCode = "20-000";
//            var voivodeship = "lubelskie";
//            var countryCode = "pl";
//            var latitude = 51.2535588;
//            var longitude = 22.5669921;
//            using (var context = GetNewSzpekContext())
//            {
//                var meassurementsController = new MeassurementsController(context);
//                var locationId = await TestsUtils.CreateSampleLocationWithSensor(
//                    context,
//                    "Michow",
//                    "a",
//                    false,
//                    new Core.Models.Address(city, street, postCode, voivodeship, countryCode, latitude, longitude));
//                var location = await context.Location.SingleAsync();
//                await CreateMeassurement(meassurementsController, location.Sensor.Code, pm10Value, pm25Value, periodFrom: DateTime.Now.AddHours(-1), periodTo, samplesQuantity: 1);

//                var actionResult = await meassurementsController.GetLastDay(locationId);
//                var value = actionResult.Value;

//                Assert.Equal(value.Meassurements.First().Pm10Value, pm10Value);
//                Assert.Equal(value.Meassurements.First().Pm25Value, pm25Value);
//                Assert.Equal(value.Meassurements.First().PeriodTo, periodTo);
//                Assert.Equal(value.Address.City, city);
//                Assert.Equal(value.Address.Street, street);
//                Assert.Equal(value.Address.PostCode, postCode);
//                Assert.Equal(value.Address.Voivodeship, voivodeship);
//                Assert.Equal(value.Address.CountryCode, countryCode);
//            }
//        }

//        private async Task CreateMeassurement(
//            MeassurementsController meassurementsController,
//            string sensorCode,
//            double pm10Value,
//            double pm25Value,
//            DateTime periodFrom,
//            DateTime periodTo,
//            int samplesQuantity)
//        {
//            var meassurementDto = new MeassurementCreate()
//            {
//                SensorCode = sensorCode,
//                Pm10Value = pm10Value,
//                Pm25Value = pm25Value,
//                PeriodFrom = periodFrom,
//                PeriodTo = periodTo,
//                SamplesQuantity = samplesQuantity
//            };

//            await meassurementsController.Post(meassurementDto);
//        }

//    }
//}
