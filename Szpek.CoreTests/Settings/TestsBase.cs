using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Szpek.Infrastructure.Models.Context;

namespace Szpek.CoreTests.Settings
{
    public class TestsBase : IDisposable
    {
        private readonly SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
        public TestsBase()
        {
            // In-memory database only exists while the connection is open
            connection.Open();

            // Create the schema in the database
            using (var context = GetNewSzpekContext())
            {
                context.Database.EnsureCreated();
            }

        }

        public void Dispose()
        {
            connection.Close();
        }

        public SzpekContext GetNewSzpekContext()
        {
            var options = new DbContextOptionsBuilder<SzpekContext>()
                    .UseSqlite(connection)
                    .Options;

            return new SzpekContext(options);
        }
    }
}
