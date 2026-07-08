using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ZoneSync.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSensorReadingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasurementTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SensorModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OutputType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SensorInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SensorModelId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorInstances_SensorModels_SensorModelId",
                        column: x => x.SensorModelId,
                        principalTable: "SensorModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SensorModelMeasurementTypes",
                columns: table => new
                {
                    SensorModelId = table.Column<int>(type: "int", nullable: false),
                    MeasurementTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorModelMeasurementTypes", x => new { x.SensorModelId, x.MeasurementTypeId });
                    table.ForeignKey(
                        name: "FK_SensorModelMeasurementTypes_MeasurementTypes_MeasurementTypeId",
                        column: x => x.MeasurementTypeId,
                        principalTable: "MeasurementTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SensorModelMeasurementTypes_SensorModels_SensorModelId",
                        column: x => x.SensorModelId,
                        principalTable: "SensorModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SensorReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SensorInstanceId = table.Column<int>(type: "int", nullable: false),
                    MeasurementTypeId = table.Column<int>(type: "int", nullable: false),
                    ReadingValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ReadingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadingTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorReadings_MeasurementTypes_MeasurementTypeId",
                        column: x => x.MeasurementTypeId,
                        principalTable: "MeasurementTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SensorReadings_SensorInstances_SensorInstanceId",
                        column: x => x.SensorInstanceId,
                        principalTable: "SensorInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZoneConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    SensorInstanceId = table.Column<int>(type: "int", nullable: false),
                    ConfiguredByUserId = table.Column<int>(type: "int", nullable: false),
                    ConfiguredAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZoneConfigurations_SensorInstances_SensorInstanceId",
                        column: x => x.SensorInstanceId,
                        principalTable: "SensorInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ZoneConfigurations_User_ConfiguredByUserId",
                        column: x => x.ConfiguredByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ZoneConfigurations_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "MeasurementTypes",
                columns: new[] { "Id", "Name", "Unit" },
                values: new object[,]
                {
                    { 1, "Soil moisture", "%" },
                    { 2, "Temperature", "C" },
                    { 3, "Water level", "%" }
                });

            migrationBuilder.InsertData(
                table: "SensorModels",
                columns: new[] { "Id", "ModelName", "OutputType", "Type" },
                values: new object[,]
                {
                    { 1, "SM-100", "Digital", "Soil" },
                    { 2, "TH-200", "Digital", "Climate" }
                });

            migrationBuilder.InsertData(
                table: "SensorModelMeasurementTypes",
                columns: new[] { "MeasurementTypeId", "SensorModelId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 3, 1 },
                    { 2, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorInstances_SensorModelId",
                table: "SensorInstances",
                column: "SensorModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorInstances_SerialNumber",
                table: "SensorInstances",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SensorModelMeasurementTypes_MeasurementTypeId",
                table: "SensorModelMeasurementTypes",
                column: "MeasurementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_MeasurementTypeId",
                table: "SensorReadings",
                column: "MeasurementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_SensorInstanceId",
                table: "SensorReadings",
                column: "SensorInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneConfigurations_ConfiguredByUserId",
                table: "ZoneConfigurations",
                column: "ConfiguredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneConfigurations_SensorInstanceId",
                table: "ZoneConfigurations",
                column: "SensorInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneConfigurations_ZoneId",
                table: "ZoneConfigurations",
                column: "ZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensorModelMeasurementTypes");

            migrationBuilder.DropTable(
                name: "SensorReadings");

            migrationBuilder.DropTable(
                name: "ZoneConfigurations");

            migrationBuilder.DropTable(
                name: "MeasurementTypes");

            migrationBuilder.DropTable(
                name: "SensorInstances");

            migrationBuilder.DropTable(
                name: "SensorModels");
        }
    }
}
