using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Szpek.Infrastructure.Models.Context;

namespace Szpek.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            //serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning) 
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()        
                .WriteTo.File(
                    GetLogsPath(),
                    rollingInterval: RollingInterval.Month)
                .CreateLogger();
            try
            {
                var appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                Log.Information($"Szpek app started, version: {appVersion}");
                var host = CreateHostBuilder(args).Build();
                MigrateDatabase(host);
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void MigrateDatabase(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SzpekContext>();

                Log.Information($"Looking for ef DB migrations");
                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    dbContext.Database.Migrate();
                    Log.Information($"DB migrated successfully");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
       .ConfigureWebHostDefaults(webBuilder =>
       {
           webBuilder.UseStartup<Startup>();
       }).UseSerilog();

        public static string GetLogsPath()
        {
            var path = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                path = "C:/log/szpek/szpek.log";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                path = "/var/log/szpek/szpek.log";
            }

            return path;
        }
    }
}
