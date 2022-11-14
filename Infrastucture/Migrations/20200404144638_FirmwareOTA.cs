using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Szpek.Infrastucture.Migrations
{
    public partial class FirmwareOTA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoardId",
                table: "Sensor",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstalledFirmwareId",
                table: "Sensor",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecommendedFirmwareId",
                table: "Sensor",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Board",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Board", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Firmware",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Content = table.Column<byte[]>(nullable: false),
                    ReleaseDateTimeUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Firmware", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SensorLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SensorId = table.Column<long>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    DateTimeUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorLog_Sensor_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sensor_BoardId",
                table: "Sensor",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensor_InstalledFirmwareId",
                table: "Sensor",
                column: "InstalledFirmwareId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensor_RecommendedFirmwareId",
                table: "Sensor",
                column: "RecommendedFirmwareId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorLog_SensorId",
                table: "SensorLog",
                column: "SensorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sensor_Board_BoardId",
                table: "Sensor",
                column: "BoardId",
                principalTable: "Board",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sensor_Firmware_InstalledFirmwareId",
                table: "Sensor",
                column: "InstalledFirmwareId",
                principalTable: "Firmware",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sensor_Firmware_RecommendedFirmwareId",
                table: "Sensor",
                column: "RecommendedFirmwareId",
                principalTable: "Firmware",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sensor_Board_BoardId",
                table: "Sensor");

            migrationBuilder.DropForeignKey(
                name: "FK_Sensor_Firmware_InstalledFirmwareId",
                table: "Sensor");

            migrationBuilder.DropForeignKey(
                name: "FK_Sensor_Firmware_RecommendedFirmwareId",
                table: "Sensor");

            migrationBuilder.DropTable(
                name: "Board");

            migrationBuilder.DropTable(
                name: "Firmware");

            migrationBuilder.DropTable(
                name: "SensorLog");

            migrationBuilder.DropIndex(
                name: "IX_Sensor_BoardId",
                table: "Sensor");

            migrationBuilder.DropIndex(
                name: "IX_Sensor_InstalledFirmwareId",
                table: "Sensor");

            migrationBuilder.DropIndex(
                name: "IX_Sensor_RecommendedFirmwareId",
                table: "Sensor");

            migrationBuilder.DropColumn(
                name: "BoardId",
                table: "Sensor");

            migrationBuilder.DropColumn(
                name: "InstalledFirmwareId",
                table: "Sensor");

            migrationBuilder.DropColumn(
                name: "RecommendedFirmwareId",
                table: "Sensor");
        }
    }
}
