using System.Threading.Tasks;
using Szpek.Core.Models;
using Szpek.Infrastructure.Models.Context;

namespace Szpek.CoreTests.Settings
{
    public static class TestsUtils
    {
        public static async Task<long> CreateSampleLocationWithSensor(SzpekContext context, string sensorCode, string sensorKey, bool isPrivate = false, Address address = null)
        {         
            var sensorId = await CreateSampleSensor(context, sensorCode, sensorKey, isPrivate);
            var location = await context.Location.AddAsync(
                Location.Create(sensorId, address ?? GetSampleEntityAddress()));
            await context.SaveChangesAsync();
            return location.Entity.Id;
        }

        public static async Task<long> CreateSampleSensor(SzpekContext context, string sensorCode, string sensorKey, bool isPrivate = false)
        {
            var sensorOwner = SensorOwner.Create("czaja", true, "Michow");
            await context.SensorOwner.AddAsync(sensorOwner);
            await context.SaveChangesAsync();
            var sensor = await context.Sensor.AddAsync(Sensor.Create(sensorCode, sensorOwner.Id, sensorKey, isPrivate));
            await context.SaveChangesAsync();
            return sensor.Entity.Id;
        }

        public static async Task<long> CreateSensorOwner(SzpekContext context, string name)
        {
            var sensorOwner = SensorOwner.Create(name, true, "Michow");
            await context.AddAsync(sensorOwner);
            await context.SaveChangesAsync();
            return sensorOwner.Id;
        }

        //change to Address object

        //public static AddressCreate GetSampleDtoAddress(string countryCode = "pl")
        //{
        //    return new AddressCreate()
        //    {
        //        City = "Michow",
        //        CountryCode = countryCode,
        //        PostCode = "21-140",
        //        Street = "Skladowska 45",
        //        Voivodeship = "lubelskie"
        //    };
        //}

        private static Address GetSampleEntityAddress()
        {
            return Address.Create("Michow", "Skladowska 45", "21-140", "lubelskie", "pl", 51.2535588, 22.5669921, 100);
        }

    }
}
