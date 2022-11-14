using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Szpek.Infrastucture.Migrations
{
    public partial class CreateGenericMeasurement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmogMeassurement_Location_LocationId",
                table: "SmogMeassurement");

            migrationBuilder.DropIndex(
                name: "IX_SmogMeassurement_LocationId",
                table: "SmogMeassurement");

            migrationBuilder.CreateTable(
                name: "WeatherMeasurement",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CelciusTemperature = table.Column<double>(nullable: false),
                    AtmosphericPreassure = table.Column<double>(nullable: false),
                    HumidityPercentage = table.Column<double>(nullable: false),
                    MeasurementDate = table.Column<DateTime>(nullable: false),
                    CreationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherMeasurement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Measurement",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LocationId = table.Column<long>(nullable: false),
                    SmogMeasurementId = table.Column<long>(nullable: false),
                    WeatherMeasurementId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measurement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Measurement_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Measurement_SmogMeassurement_SmogMeasurementId",
                        column: x => x.SmogMeasurementId,
                        principalTable: "SmogMeassurement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Measurement_WeatherMeasurement_WeatherMeasurementId",
                        column: x => x.WeatherMeasurementId,
                        principalTable: "WeatherMeasurement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Measurement_LocationId",
                table: "Measurement",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Measurement_SmogMeasurementId",
                table: "Measurement",
                column: "SmogMeasurementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Measurement_WeatherMeasurementId",
                table: "Measurement",
                column: "WeatherMeasurementId",
                unique: true);
        }
            
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Measurement");

            migrationBuilder.DropTable(
                name: "WeatherMeasurement");

            migrationBuilder.CreateIndex(
                name: "IX_SmogMeassurement_LocationId",
                table: "SmogMeassurement",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_SmogMeassurement_Location_LocationId",
                table: "SmogMeassurement",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
