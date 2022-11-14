using System;
using System.Collections.Generic;
using System.Text;

namespace Szpek.CoreUnitTests
{
    class SensorRepostitoryTests
    {
        //[Fact]
        //public async Task CreateSensor_ShouldBeCreated()
        //{
        //    using (var context = GetNewSzpekContext())
        //    {
        //        var sensorsController = new SensorsController(context);
        //        var ownerId = await TestsUtils.CreateSensorOwner(context, "czaja");

        //        var sensorDto = new SensorCreate()
        //        { Code = "Michow", OwnerId = ownerId, PublicKey = "", IsPrivate = false };

        //        await sensorsController.Post(sensorDto);
        //    }

        //    using (var context = GetNewSzpekContext())
        //    {
        //        Assert.Equal(1, context.Sensor.Count());
        //        Assert.Equal("Michow", context.Sensor.Single().Code);
        //    }
        //}


        //[Fact]
        //empty list
        //public async Task GetSensorsWhenNoSensors_ShouldBeReturned()
        //{
        //    using (var context = GetNewSzpekContext())
        //    {
        //        var sensorsController = new SensorsController(context);

        //        var result = await sensorsController.Get();

        //        Assert.IsType<ActionResult<IEnumerable<SensorRead>>>(result);
        //    }
        //}

        //[Fact]
        //one
        //public async Task GetSensorsWhenSensorExists_ShouldBeReturned()
        //{
        //    using (var context = GetNewSzpekContext())
        //    {
        //        var sensorsController = new SensorsController(context);
        //        var ownerId = await TestsUtils.CreateSensorOwner(context, "czaja");
        //        var sensorDto = new SensorCreate()
        //        { Code = "Michow", OwnerId = ownerId, PublicKey = "", IsPrivate = false };
        //        await sensorsController.Post(sensorDto);

        //        var result = await sensorsController.Get();

        //        Assert.IsType<ActionResult<IEnumerable<SensorRead>>>(result);
        //    }
        //}

        //[Fact]
        //public async Task GetSensorByIdWhenNoSensorExist_ShouldBeNotFound()
        //null
        //{
        //    using (var context = GetNewSzpekContext())
        //    {
        //        var sensorsController = new SensorsController(context);  

        //        var actionResult = await sensorsController.Get(1);

        //        Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        //    }
        //}

        //[Fact]
        //public async Task GetSensorByIdWhenSensorExists_ShouldBeReturned()
        //returned
        //{
        //    var sensorCode = "Michow";
        //    using (var context = GetNewSzpekContext())
        //    {
        //        var sensorsController = new SensorsController(context);
        //        var ownerId = await TestsUtils.CreateSensorOwner(context, "czaja");
        //        var sensorDto = new SensorCreate()
        //        { Code = sensorCode, OwnerId = ownerId, PublicKey = "", IsPrivate = false };
        //        await sensorsController.Post(sensorDto);

        //        var sensorId = context.Sensor.Single().Id;
        //        var actionResult = await sensorsController.Get(sensorId);

        //        Assert.Equal(sensorCode, actionResult.Value.Code);
        //    }
        //}      
    }
}
