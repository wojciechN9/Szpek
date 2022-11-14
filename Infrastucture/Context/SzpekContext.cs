using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Szpek.Core.Models;

namespace Szpek.Infrastructure.Models.Context
{
    public class SzpekContext : IdentityDbContext
    {
        public SzpekContext(DbContextOptions<SzpekContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Sensor>()
                .HasOne(s => s.SensorOwner)
                .WithMany(so => so.Sensors);

            modelBuilder.Entity<Sensor>()
                .Property(s => s.Code)
                .IsRequired()
                .HasMaxLength(45);
            modelBuilder.Entity<Sensor>()
                .HasIndex(s => s.Code)
                .IsUnique();

            modelBuilder.Entity<Sensor>()
                .Property(s => s.PublicKey)
                .IsRequired()
                .HasMaxLength(2000);

            modelBuilder.Entity<Sensor>()
                .Property(s => s.IsPrivate)
                .IsRequired();

            modelBuilder.Entity<SensorOwner>()
                .Property(s => s.Name)
                .IsRequired();

            modelBuilder.Entity<Address>()
              .Property(a => a.CountryCode)
              .HasMaxLength(2);

            modelBuilder.Entity<Location>()
                .HasOne(l => l.Sensor)
                .WithMany(s => s.Locations);

            modelBuilder.Entity<Location>()
                .HasOne(l => l.Address)
                .WithOne(a => a.Location)
                .HasForeignKey<Address>(a => a.LocationId);

            modelBuilder.Entity<Measurement>()
                .HasOne(m => m.Location)
                .WithMany(l => l.Meassurements);

            modelBuilder.Entity<WeatherMeasurement>()
                .HasOne(m => m.Measurement)
                .WithOne(m => m.WeatherMeasurement)
                .HasForeignKey<Measurement>(m => m.WeatherMeasurementId);

            modelBuilder.Entity<SmogMeasurement>()
                .HasOne(m => m.Measurement)
                .WithOne(m => m.SmogMeasurement)
                .HasForeignKey<Measurement>(m => m.SmogMeasurementId);

            modelBuilder.Entity<SmogMeasurement>()
                .Property(m => m.Pm1Value)
                .HasDefaultValue(0)
                .IsRequired();

            modelBuilder.Entity<SmogMeasurement>()
                .Property(x => x.AirQuality).HasConversion<int>();
            modelBuilder.Entity<SmogMeasurement>()
                .Property(x => x.Pm10Quality).HasConversion<int>();
            modelBuilder.Entity<SmogMeasurement>()
                .Property(x => x.Pm25Quality).HasConversion<int>();

            modelBuilder.Entity<AirQualityLevel>()
                .Property(x => x.AirQuality).HasConversion<int>();
            modelBuilder.Entity<AirQualityLevel>()
               .Property(x => x.PollutionType).HasConversion<int>();

            modelBuilder.Entity<Board>()
                .Property(x => x.Name).HasMaxLength(50).IsRequired();

            modelBuilder.Entity<Firmware>(f =>
            {
                f.Property(x => x.Name).HasMaxLength(50).IsRequired();
                f.Property(x => x.Content).IsRequired();
                f.Property(x => x.ReleaseDateTimeUtc).IsRequired();
            });

            modelBuilder.Entity<SensorLog>()
                .HasOne(x => x.Sensor).WithMany(x => x.Logs);

            GetDatetimesAsUtc(modelBuilder);

            SeedData(modelBuilder);
        }

        private static void GetDatetimesAsUtc(ModelBuilder modelBuilder)
        {
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                            v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                        property.SetValueConverter(dateTimeConverter);
                }
            }
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AirQualityLevel>().HasData(
                new AirQualityLevel(1, PollutionType.PM10, AirQuality.VeryGood, 0, 21, new System.DateTime(2019, 06, 28)),
                new AirQualityLevel(2, PollutionType.PM10, AirQuality.Good, 21, 61, new System.DateTime(2019, 06, 28)),
                new AirQualityLevel(3, PollutionType.PM10, AirQuality.Ok, 61, 101, new System.DateTime(2019, 06, 28)),
                new AirQualityLevel(4, PollutionType.PM10, AirQuality.Poor, 101, 141, new System.DateTime(2019, 06, 28)),
                new AirQualityLevel(5, PollutionType.PM10, AirQuality.Bad, 141, 201, new System.DateTime(2019, 06, 28)),
                new AirQualityLevel(6, PollutionType.PM10, AirQuality.VeryBad, 201, 3000, new System.DateTime(2019, 06, 28)),
                new AirQualityLevel(7, PollutionType.PM25, AirQuality.VeryGood, 0, 13, new System.DateTime(2019, 06, 28)),
                new AirQualityLevel(8, PollutionType.PM25, AirQuality.Good, 13, 37, new System.DateTime(2019, 06, 28)),
                new AirQualityLevel(9, PollutionType.PM25, AirQuality.Ok, 37, 61, new System.DateTime(2019, 06, 28)),
                new AirQualityLevel(10, PollutionType.PM25, AirQuality.Poor, 61, 85, new System.DateTime(2019, 06, 28)),
                new AirQualityLevel(11, PollutionType.PM25, AirQuality.Bad, 85, 121, new System.DateTime(2019, 06, 28)),
                new AirQualityLevel(12, PollutionType.PM25, AirQuality.VeryBad, 121, 3000, new System.DateTime(2019, 06, 28)));
        }

        public DbSet<Sensor> Sensor { get; set; }

        public DbSet<Location> Location { get; set; }

        public DbSet<AirQualityLevel> AirQualityLevel { get; set; }

        public DbSet<SensorOwner> SensorOwner { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<Measurement> Measurement { get; set; }

        public DbSet<SmogMeasurement> SmogMeassurement { get; set; }

        public DbSet<Firmware> Firmware { get; set; }

        public DbSet<Board> Board { get; set; }

        public DbSet<SensorLog> SensorLog { get; set; }
    }
}
