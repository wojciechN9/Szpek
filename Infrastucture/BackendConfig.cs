using System;

namespace Szpek.Infrastructure
{
    public static class BackendConfig
    {
        public static string DBConnectionString = Environment.GetEnvironmentVariable("szpek_dbConnectionString");

        public static string JwtSecret = Environment.GetEnvironmentVariable("szpek_jwtSecret");

        public static string AdminPassword = Environment.GetEnvironmentVariable("szpek_adminPassword");

        public static string AdminEmail = Environment.GetEnvironmentVariable("szpek_adminMail");

        public static string NoreplyEmailProvider = Environment.GetEnvironmentVariable("szpek_noreplyMailProvider");

        public static string NoreplyEmail = Environment.GetEnvironmentVariable("szpek_noreplyMail");

        public static string NoreplyEmailPassword = Environment.GetEnvironmentVariable("szpek_noreplyMailPassword");
    }
}
