using Microsoft.EntityFrameworkCore.Migrations;

namespace Szpek.Infrastucture.Migrations
{
    public partial class MakeSensorOwnerFieldsNotNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SensorOwner",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SensorOwner",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
