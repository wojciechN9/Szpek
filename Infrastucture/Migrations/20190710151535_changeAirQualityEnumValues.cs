using Microsoft.EntityFrameworkCore.Migrations;

namespace Szpek.Infrastucture.Migrations
{
    public partial class changeAirQualityEnumValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 1L,
                column: "AirQuality",
                value: 0);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 2L,
                column: "AirQuality",
                value: 1);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 3L,
                column: "AirQuality",
                value: 2);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 4L,
                column: "AirQuality",
                value: 3);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 5L,
                column: "AirQuality",
                value: 4);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 6L,
                column: "AirQuality",
                value: 5);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 7L,
                column: "AirQuality",
                value: 0);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 8L,
                column: "AirQuality",
                value: 1);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 9L,
                column: "AirQuality",
                value: 2);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 10L,
                column: "AirQuality",
                value: 3);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 11L,
                column: "AirQuality",
                value: 4);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 12L,
                column: "AirQuality",
                value: 5);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 1L,
                column: "AirQuality",
                value: 1);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 2L,
                column: "AirQuality",
                value: 2);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 3L,
                column: "AirQuality",
                value: 3);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 4L,
                column: "AirQuality",
                value: 4);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 5L,
                column: "AirQuality",
                value: 5);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 6L,
                column: "AirQuality",
                value: 6);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 7L,
                column: "AirQuality",
                value: 1);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 8L,
                column: "AirQuality",
                value: 2);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 9L,
                column: "AirQuality",
                value: 3);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 10L,
                column: "AirQuality",
                value: 4);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 11L,
                column: "AirQuality",
                value: 5);

            migrationBuilder.UpdateData(
                table: "AirQualityLevel",
                keyColumn: "Id",
                keyValue: 12L,
                column: "AirQuality",
                value: 6);
        }
    }
}
