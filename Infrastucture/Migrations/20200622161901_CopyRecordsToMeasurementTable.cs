using Microsoft.EntityFrameworkCore.Migrations;

namespace Szpek.Infrastucture.Migrations
{
    public partial class CopyRecordsToMeasurementTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                //on server table has Measurement name - idk why there is such a diference
                @"
                    INSERT INTO Measurement (LocationId, SmogMeasurementId) 
                    SELECT LocationId, Id FROM SmogMeassurement ORDER BY Id;
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            "TRUNCATE TABLE Measurement;");
        }
    }
}
