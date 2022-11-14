using Microsoft.EntityFrameworkCore.Migrations;

namespace Szpek.Infrastucture.Migrations
{
    public partial class ChangeNameToSmogMeasurement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meassurement_Location_LocationId",
                table: "Meassurement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meassurement",
                table: "Meassurement");

            migrationBuilder.RenameTable(
                name: "Meassurement",
                newName: "SmogMeassurement");

            migrationBuilder.RenameIndex(
                name: "IX_Meassurement_LocationId",
                table: "SmogMeassurement",
                newName: "IX_SmogMeassurement_LocationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SmogMeassurement",
                table: "SmogMeassurement",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SmogMeassurement_Location_LocationId",
                table: "SmogMeassurement",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmogMeassurement_Location_LocationId",
                table: "SmogMeassurement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SmogMeassurement",
                table: "SmogMeassurement");

            migrationBuilder.RenameTable(
                name: "SmogMeassurement",
                newName: "Meassurement");

            migrationBuilder.RenameIndex(
                name: "IX_SmogMeassurement_LocationId",
                table: "Meassurement",
                newName: "IX_Meassurement_LocationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meassurement",
                table: "Meassurement",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meassurement_Location_LocationId",
                table: "Meassurement",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
