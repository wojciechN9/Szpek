using Microsoft.Extensions.DependencyInjection;

namespace Szpek.Infrastructure.SensorContext
{
    public static class ServiceCollectionSensorContextExtensions
    {
        public static IServiceCollection AddSensorContext(this IServiceCollection services)
        {
            services.AddScoped<BasicSensorContext>();
            services.AddTransient<ISensorContext>(provider => provider.GetService<BasicSensorContext>());

            return services;
        }
    }
}
