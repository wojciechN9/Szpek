using System;

namespace Szpek.Api
{
    public static class BackendConfig
    {
        public static string DBConnectionString = Environment.GetEnvironmentVariable("szpek_dbConnectionString");

        public static string JwtSecret = Environment.GetEnvironmentVariable("szpek_jwtSecret");

        public static string AdminPassword = Environment.GetEnvironmentVariable("szpek_adminPassword");

        public static string AdminEmail = Environment.GetEnvironmentVariable("szpek_adminMail");

    }
}
