using Microsoft.EntityFrameworkCore.Migrations;

namespace Szpek.Infrastucture.Migrations
{
    public partial class AddSensorOwnerIdToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SensorOwnerId",
                table: "AspNetUsers",
                column: "SensorOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SensorOwner_SensorOwnerId",
                table: "AspNetUsers",
                column: "SensorOwnerId",
                principalTable: "SensorOwner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SensorOwner_SensorOwnerId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SensorOwnerId",
                table: "AspNetUsers");
        }
    }
}
