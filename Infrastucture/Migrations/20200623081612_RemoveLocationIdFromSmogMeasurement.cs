using Microsoft.EntityFrameworkCore.Migrations;

namespace Szpek.Infrastucture.Migrations
{
    public partial class RemoveLocationIdFromSmogMeasurement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "SmogMeassurement");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "SmogMeassurement",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
