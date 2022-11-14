//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Szpek.Api.Controllers;
//using Szpek.Application.Location;
//using Szpek.Core.Models;
//using Szpek.IntegrationTests.Settings;
//using Xunit;

//namespace Szpek.ApiUnitTests
//{
//    public class LocationsControllerTests : TestsBase
//    {
//        public LocationsControllerTests() : base() { }

//        [Fact]
//        public async Task CreateLocation_WithNotExistedSensorId_UnprocessableEntityShouldBeReturned()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);

//                var locationDto = new LocationCreate()
//                {
//                    SensorId = default,
//                    Address = TestsUtils.GetSampleDtoAddress()
//                };

//                var actionResult = await locationsController.NewSensorLocation(locationDto);

//                Assert.IsType<UnprocessableEntityObjectResult>(actionResult);
//            }
//        }

//        [Fact]
//        public async Task CreateLocation_ShouldBeCreatedAndActive()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);

//                var sensorId = await TestsUtils
//                    .CreateSampleSensor(context, sensorCode: "Lublin", sensorKey: "a");

//                var locationDto = new LocationCreate()
//                {
//                    SensorId = sensorId,
//                    Address = TestsUtils.GetSampleDtoAddress()
//                };

//                await locationsController.NewSensorLocation(locationDto);
//            }

//            using (var context = GetNewSzpekContext())
//            {
//                Assert.Equal(1, context.Location.Count());
//                Assert.True(context.Location.Single().IsActive);
//            }
//        }

//        [Fact]
//        public async Task CreateLocation_OldLocationShouldBeInActive()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);
//                await TestsUtils.CreateSampleLocationWithSensor(context, sensorCode: "Michów", sensorKey: "a");
//                var oldLocation = await context.Location
//                    .FirstAsync();
//                var newlocationDto = new LocationCreate()
//                {
//                    SensorId = oldLocation.SensorId,
//                    Address = TestsUtils.GetSampleDtoAddress()
//                };

//                await locationsController.NewSensorLocation(newlocationDto);

//                Assert.False(oldLocation.IsActive);
//            }
//        }

//        [Fact]
//        public async Task CreateLocation_ShouldReturnResult()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);
//                var sensorId = await TestsUtils.CreateSampleSensor(context, "a", "ble");
//                var locationDto = new LocationCreate() { Address = new Application.Address.AddressCreate(), SensorId = sensorId };
//                var result = await locationsController.NewSensorLocation(locationDto);

//                Assert.IsType<OkObjectResult>(result);
//            }
//        }

//        [Fact]
//        public async Task GetLocationsWhenNoLocations_ShouldBeReturned()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);

//                var result = await locationsController.Get();

//                Assert.IsType<ActionResult<IEnumerable<LocationRead>>>(result);
//            }
//        }

//        [Fact]
//        public async Task GetLocationsWhenLocationExists_ShouldBeReturned()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);
//                var sensorId = await TestsUtils.CreateSampleSensor(context, "a", "ble");
//                var locationDto = new LocationCreate() { Address = new Application.Address.AddressCreate(), SensorId = sensorId };
//                await locationsController.NewSensorLocation(locationDto);

//                var result = await locationsController.Get();

//                Assert.IsType<ActionResult<IEnumerable<LocationRead>>>(result);
//            }
//        }

//        [Fact]
//        public async Task GetLocationById_ShouldBeReturned()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);
//                var sensorId = await TestsUtils.CreateSampleSensor(context, "a", "ble");
//                var location = new Location(sensorId, new Address("a", "a", "a", "a", "a", 1, 1));
//                await context.Location.AddAsync(location);
//                await context.SaveChangesAsync();

//                var result = await locationsController.Get(location.Id);

//                Assert.IsType<ActionResult<LocationDetailsRead>>(result);
//            }
//        }

//        [Fact]
//        public async Task GetLocationByIdWhenNoLocationExist_ShouldBeNotFoundReturned()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);

//                var actionResult = await locationsController.Get(1);

//                Assert.IsType<NotFoundObjectResult>(actionResult.Result);
//            }
//        }

//        [Fact]
//        public async Task EnableOldLocationWhenLocationNotExist_ShouldBeNotFoundThrown()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);

//                var actionResult = await locationsController.EnableOldLocation(
//                    locationId: default,
//                    sensorId: default);

//                Assert.IsType<NotFoundObjectResult>(actionResult);
//            }
//        }

//        [Fact]
//        public async Task EnableOldLocationWhenSensorNotExist_ShouldBeNotFoundThrown()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);
//                await TestsUtils.CreateSampleLocationWithSensor(context, sensorCode: "Michów", sensorKey: "a");
//                var locationId = (await context.Location.SingleAsync()).Id;

//                var actionResult = await locationsController.EnableOldLocation(
//                    locationId,
//                    sensorId: default);

//                Assert.IsType<NotFoundObjectResult>(actionResult);
//            }
//        }

//        [Fact]
//        public async Task EnableOldLocationWhenOldLocationIsActive_ShouldBeNotFoundThrown()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);
//                var sensorId = await TestsUtils
//                    .CreateSampleSensor(context, sensorCode: "Lublin", sensorKey: "a");
//                await TestsUtils.CreateSampleLocationWithSensor(context, sensorCode: "Michów", sensorKey: "b");
//                var locationId = (await context.Location.SingleAsync()).Id;

//                var actionResult = await locationsController.EnableOldLocation(
//                    locationId,
//                    sensorId);

//                Assert.IsType<NotFoundObjectResult>(actionResult);
//            }
//        }

//        [Fact]
//        public async Task EnableOldLocation_ShouldBeActivatedAndHadNewSensorId()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);
//                await TestsUtils.CreateSampleLocationWithSensor(context, sensorCode: "Michów", sensorKey: "a");
//                var locationId = (await context.Location.SingleAsync()).Id;
//                var sensorId = await TestsUtils
//                    .CreateSampleSensor(context, sensorCode: "Lublin01", sensorKey: "b");

//                var location = await context.Location
//                    .FirstOrDefaultAsync(l => l.Id == locationId);
//                location.Deactivate();
//                await context.SaveChangesAsync();

//                var actionResult = await locationsController.EnableOldLocation(
//                    locationId,
//                    sensorId);

//                Assert.IsType<NoContentResult>(actionResult);
//                Assert.True(location.IsActive);
//                Assert.Equal(location.SensorId, sensorId);
//            }
//        }

//        [Fact]
//        public async Task EnableOldLocation_CurrentLocationShouldBeInActive()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);

//                var oldLocationId = await TestsUtils
//                    .CreateSampleLocationWithSensor(context, sensorCode: "Lublin", sensorKey: "b");
//                var oldLocation = await context.Location
//                    .FirstAsync(l => l.Id == oldLocationId);
//                oldLocation.Deactivate();
//                await context.SaveChangesAsync();

//                var currentLocationId = await TestsUtils
//                    .CreateSampleLocationWithSensor(context, sensorCode: "Michów", sensorKey: "a");
//                var currentLocation = await context.Location
//                    .FirstAsync(l => l.Id == currentLocationId);

//                await locationsController.EnableOldLocation(oldLocationId, currentLocation.SensorId);

//                Assert.False(currentLocation.IsActive);
//            }
//        }

//        [Fact]
//        public async Task ChangeStreetAddressWhenNoLocation_NotFoundShouldBeReturned()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var locationsController = new LocationsController(context);

//                var actionResult = await locationsController.ChangeStreetAddress(
//                    locationId: default,
//                    streetAddress: default);

//                Assert.IsType<NotFoundObjectResult>(actionResult);
//            }
//        }

//        [Fact]
//        public async Task ChangeStreetAddress_AddressShouldBeChanged()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var newStreetAddress = "Składowska 18";
//                var locationsController = new LocationsController(context);
//                var sensorId = await TestsUtils
//                    .CreateSampleSensor(context, sensorCode: "Lublin01", sensorKey: "a");
//                var location = await context.Location.AddAsync(
//                new Location(sensorId, new Address("lublin", street: "lubartowska", "21-100", "lubelskie", "pl", 22.5669921, 51.2535588)));
//                await context.SaveChangesAsync();

//                var actionResult = await locationsController.ChangeStreetAddress(
//                    locationId: location.Entity.Id,
//                    streetAddress: newStreetAddress);

//                Assert.IsType<NoContentResult>(actionResult);
//                Assert.Equal(location.Entity.Address.Street, newStreetAddress);
//            }
//        }


//    }
//}
