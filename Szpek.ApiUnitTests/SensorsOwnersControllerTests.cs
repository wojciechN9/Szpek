//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using Szpek.Api.Controllers;
//using Szpek.Application.SensorOwner;
//using Szpek.Core.Models;
//using Szpek.IntegrationTests.Settings;
//using Xunit;

//namespace Szpek.ApiUnitTests
//{
//    public class SensorsOwnersControllerTests : TestsBase
//    {
//        readonly Mock<IUserStore<User>> _userStore;
//        readonly Mock<UserManager<User>> _userManager;

//        public SensorsOwnersControllerTests() : base()
//        {
//            _userStore = new Mock<IUserStore<User>>();
//            _userManager = new Mock<UserManager<User>>(_userStore.Object, null, null, null, null, null, null, null, null);
//        }

//        [Fact]
//        public async void PostSensorOwner_ShouldBeCreated()
//        {
//            _userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
//                .Returns(Task.FromResult(new User() { Id = "1" }));

//            using (var context = GetNewSzpekContext())
//            {
//                var sensorsOwnersController = new SensorsOwnersController(context, _userManager.Object);
//                var sensorOwnerDto = new SensorOwnerCreate() { UserId = "a", Name = "kamil", IsCompany = false };

//                var result = await sensorsOwnersController.Post(sensorOwnerDto);
//            }
//            using (var context = GetNewSzpekContext())
//            {
//                Assert.Equal(1, await context.SensorOwner.CountAsync());
//                Assert.Equal("", (await context.SensorOwner.SingleAsync()).Address);
//            }
//        }

//        [Fact]
//        public async void PostSensorOwner_ShouldBeActionResultLongReturned()
//        {
//            _userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
//               .Returns(Task.FromResult(new User() { Id = "1" }));

//            using (var context = GetNewSzpekContext())
//            {
//                var sensorsOwnersController = new SensorsOwnersController(context, _userManager.Object);
//                var sensorOwnerDto = new SensorOwnerCreate() { UserId = "a", Name = "kamil", IsCompany = false };

//                var result = await sensorsOwnersController.Post(sensorOwnerDto);

//                Assert.IsType<ActionResult<long>>(result);
//            }
//        }

//        [Fact]
//        public async void PostSensorOwner_WhenUserNotExist_UnprocessableEntityShouldBeReturned()
//        {
//            _userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
//               .Returns(Task.FromResult<User>(null));

//            using (var context = GetNewSzpekContext())
//            {
//                var sensorsOwnersController = new SensorsOwnersController(context, _userManager.Object);
//                var sensorOwnerDto = new SensorOwnerCreate() { UserId = "a", Name = "kamil", IsCompany = false };

//                var result = await sensorsOwnersController.Post(sensorOwnerDto);

//                Assert.IsType<UnprocessableEntityObjectResult>(result.Result);
//            }
//        }

//        [Fact]
//        public async void PostSensorOwner_WhenUserWithSensorOwenerExist_UnprocessableEntityShouldBeReturned()
//        {
//            _userManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
//               .Returns(Task.FromResult(await GetUserWithOwnerId()));

//            using (var context = GetNewSzpekContext())
//            {
//                var sensorsOwnersController = new SensorsOwnersController(context, _userManager.Object);
//                var sensorOwnerDto = new SensorOwnerCreate() { UserId = "a", Name = "kamil", IsCompany = false };

//                var result = await sensorsOwnersController.Post(sensorOwnerDto);

//                Assert.IsType<UnprocessableEntityObjectResult>(result.Result);
//            }
//        }

//        [Fact]
//        public async Task GetSensorsOwnersWhenNoSensorsOwners_ShouldBeReturned()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var sensorsOwnersController = new SensorsOwnersController(context, _userManager.Object);

//                var result = await sensorsOwnersController.Get();

//                Assert.IsType<ActionResult<IEnumerable<SensorOwnerRead>>>(result);
//            }
//        }

//        [Fact]
//        public async Task GetSensorsOwnerById_ShouldBeReturned()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var sensorOwnerName = "a";

//                var sensorsOwnersController = new SensorsOwnersController(context, _userManager.Object);

//                var sensorOwner = await context.AddAsync(new SensorOwner(sensorOwnerName, false));

//                await context.SaveChangesAsync();

//                var result = await sensorsOwnersController.Get(sensorOwner.Entity.Id);

//                Assert.IsType<OkObjectResult>(result.Result);
//            }
//        }

//        [Fact]
//        public async Task GetSensorsOwnerById_WhenNotExist_ShouldBeUnprocessableEntityReturned()
//        {
//            using (var context = GetNewSzpekContext())
//            {
//                var sensorsOwnersController = new SensorsOwnersController(context, _userManager.Object);

//                var result = await sensorsOwnersController.Get(It.IsAny<long>());

//                Assert.IsType<UnprocessableEntityObjectResult>(result.Result);
//            }
//        }

//        private async Task<User> GetUserWithOwnerId()
//        {
//            var user = new User();
//            await user.AddSensorOwnerAsync(_userManager.Object, 1);

//            return user;
//        }
//    }
//}
