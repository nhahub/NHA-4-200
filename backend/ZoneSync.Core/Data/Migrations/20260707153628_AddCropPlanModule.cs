using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ZoneSync.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCropPlanModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Crops",
                columns: table => new
                {
                    CropId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CropName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CropSeason = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CropCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IrrigationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crops", x => x.CropId);
                });

            migrationBuilder.CreateTable(
                name: "CropPlans",
                columns: table => new
                {
                    CropPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CropId = table.Column<int>(type: "int", nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    PlantingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PredictedHarvestTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualHarvestTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropPlans", x => x.CropPlanId);
                    table.ForeignKey(
                        name: "FK_CropPlans_Crops_CropId",
                        column: x => x.CropId,
                        principalTable: "Crops",
                        principalColumn: "CropId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CropPlans_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GrowthStages",
                columns: table => new
                {
                    StageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CropId = table.Column<int>(type: "int", nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StageOrder = table.Column<int>(type: "int", nullable: false),
                    StageDuration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthStages", x => x.StageId);
                    table.ForeignKey(
                        name: "FK_GrowthStages_Crops_CropId",
                        column: x => x.CropId,
                        principalTable: "Crops",
                        principalColumn: "CropId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StageInformations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    CropPlanId = table.Column<int>(type: "int", nullable: false),
                    PredictedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PredictedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DelayDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StageStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageInformations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StageInformations_CropPlans_CropPlanId",
                        column: x => x.CropPlanId,
                        principalTable: "CropPlans",
                        principalColumn: "CropPlanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StageInformations_GrowthStages_StageId",
                        column: x => x.StageId,
                        principalTable: "GrowthStages",
                        principalColumn: "StageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StageRequirements",
                columns: table => new
                {
                    ReqId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    ReqName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MaxValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ApplicablePeriod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DefaultVerificationAfterHours = table.Column<int>(type: "int", nullable: false),
                    IsChosenByUser = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageRequirements", x => x.ReqId);
                    table.ForeignKey(
                        name: "FK_StageRequirements_GrowthStages_StageId",
                        column: x => x.StageId,
                        principalTable: "GrowthStages",
                        principalColumn: "StageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CheckRequirements",
                columns: table => new
                {
                    CheckId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequirementId = table.Column<int>(type: "int", nullable: false),
                    CropPlanId = table.Column<int>(type: "int", nullable: false),
                    CheckedValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    LastCheckDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextCheckDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsSatisfied = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckRequirements", x => x.CheckId);
                    table.ForeignKey(
                        name: "FK_CheckRequirements_CropPlans_CropPlanId",
                        column: x => x.CropPlanId,
                        principalTable: "CropPlans",
                        principalColumn: "CropPlanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckRequirements_StageRequirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "StageRequirements",
                        principalColumn: "ReqId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Crops",
                columns: new[] { "CropId", "CropCategory", "CropName", "CropSeason", "IrrigationType" },
                values: new object[,]
                {
                    { 1, "Vegetable", "Tomato", "Summer", "Drip" },
                    { 2, "Grain", "Wheat", "Winter", "Sprinkler" }
                });

            migrationBuilder.InsertData(
                table: "GrowthStages",
                columns: new[] { "StageId", "CropId", "StageDuration", "StageName", "StageOrder" },
                values: new object[,]
                {
                    { 1, 1, 10, "Germination", 1 },
                    { 2, 1, 30, "Vegetative", 2 },
                    { 3, 1, 20, "Harvest", 3 },
                    { 4, 2, 25, "Tillering", 1 }
                });

            migrationBuilder.InsertData(
                table: "StageRequirements",
                columns: new[] { "ReqId", "ApplicablePeriod", "DefaultVerificationAfterHours", "IsChosenByUser", "MaxValue", "MinValue", "ReqName", "StageId" },
                values: new object[,]
                {
                    { 1, "Daily", 24, true, 70m, 45m, "Soil moisture", 1 },
                    { 2, "Daily", 24, true, 30m, 18m, "Temperature", 2 },
                    { 3, "Weekly", 48, true, 60m, 30m, "Water level", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckRequirements_CropPlanId",
                table: "CheckRequirements",
                column: "CropPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckRequirements_RequirementId",
                table: "CheckRequirements",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_CropPlans_CropId",
                table: "CropPlans",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_CropPlans_ZoneId",
                table: "CropPlans",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthStages_CropId",
                table: "GrowthStages",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_StageInformations_CropPlanId",
                table: "StageInformations",
                column: "CropPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_StageInformations_StageId",
                table: "StageInformations",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_StageRequirements_StageId",
                table: "StageRequirements",
                column: "StageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckRequirements");

            migrationBuilder.DropTable(
                name: "StageInformations");

            migrationBuilder.DropTable(
                name: "StageRequirements");

            migrationBuilder.DropTable(
                name: "CropPlans");

            migrationBuilder.DropTable(
                name: "GrowthStages");

            migrationBuilder.DropTable(
                name: "Crops");
        }
    }
}
