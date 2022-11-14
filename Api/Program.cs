using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

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
                CreateWebHostBuilder(args).Build().Run();
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
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            })
            .UseSerilog();

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
