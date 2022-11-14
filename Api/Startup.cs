using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Szpek.Api.Middleware;
using Szpek.Core.Interfaces;
using Szpek.Core.Models;
using Szpek.Data.Repositories;
using Szpek.Infrastructure.Authorization;
using Szpek.Infrastructure.Email;
using Szpek.Infrastructure.Models.Context;
using Szpek.Infrastructure.SensorContext;

namespace Szpek.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //cors
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                builder =>
                {
                    builder
                    .WithOrigins("http://37.47.15.112:4200", //michow
                                        "http://188.127.9.44:4200", //lublin
                                        "http://localhost:4200",
                                        "https://szpek.pl",
                                        "http://szpek.pl", //this and 2 below are redundtat if .htaccess redirect correctly
                                        "https://www.szpek.pl",
                                        "http://www.szpek.pl",
                                        "https://vps737309.ovh.net")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                });
            });

            // identity authorization
            services.AddIdentity<User, IdentityRole>()
               .AddEntityFrameworkStores<SzpekContext>()
               .AddDefaultTokenProviders();

            // configure jwt authentication
            var key = Encoding.ASCII.GetBytes(BackendConfig.JwtSecret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddDbContext<SzpekContext>(opt =>
                opt.UseMySql(BackendConfig.DBConnectionString));

            services.AddControllers();

            services.AddTransient<IUserVerifier, UserVerifier>();
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddTransient<IAirQualityLevelRepository, AirQualityLevelRepository>();
            services.AddTransient<IFirmwareRepository, FirmwareRepository>();
            services.AddTransient<ILocationRepository, LocationRepository>();
            services.AddTransient<IMeassurementRepository, MeassurementRepository>();
            services.AddTransient<ISensorLogRepository, SensorLogRepository>();
            services.AddTransient<ISensorOwnerRepository, SensorOwnerRepository>();
            services.AddTransient<ISensorRepository, SensorRepository>();

            services.AddSensorContext();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "api";
            });

            app.UseCors();

            app.UseRouting();

            CreateAdminAndRoles(serviceProvider).Wait();
            app.UseSensorAuthentication("/sensorApi");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });        
        }

        private async Task CreateAdminAndRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();
            string[] roleNames = { RoleNames.Admin, RoleNames.SensorOwner };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: 
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // find the user with the admin email 
            var _user = await UserManager.FindByEmailAsync(BackendConfig.AdminEmail);

            // check if the user exists
            if (_user == null)
            {
                //Here you could create the super admin who will maintain the web app
                var poweruser = new User
                {
                    UserName = "admin",
                    Email = BackendConfig.AdminEmail,
                };
                string adminPassword = BackendConfig.AdminPassword;

                var createPowerUser = await UserManager.CreateAsync(poweruser, adminPassword);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(poweruser, RoleNames.Admin);
                    await UserManager.AddToRoleAsync(poweruser, RoleNames.SensorOwner);
                }
            }
        }
    }
}
