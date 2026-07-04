using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZoneSync.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFarmZoneModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntityActivityLog",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActionAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityActivityLog", x => x.ActivityId);
                    table.CheckConstraint("CHK_EntityActivityLog_ActionType", "[ActionType] IN ('Create', 'Update', 'Delete')");
                    table.CheckConstraint("CHK_EntityActivityLog_EntityType", "[EntityType] IN ('Zone', 'CropPlan', 'Task')");
                    table.ForeignKey(
                        name: "FK_EntityActivityLog_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Farm",
                columns: table => new
                {
                    FarmId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerUserId = table.Column<int>(type: "int", nullable: false),
                    FarmName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FarmLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoilType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TotalArea = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    NoOfZones = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    SoftDeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farm", x => x.FarmId);
                    table.ForeignKey(
                        name: "FK_Farm_User_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Zone",
                columns: table => new
                {
                    ZoneId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoneName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ZoneArea = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ZoneStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    SupervisorId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    SoftDeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FarmId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zone", x => x.ZoneId);
                    table.CheckConstraint("CHK_Zone_Status", "[ZoneStatus] IN ('Available', 'Planted', 'Inactive')");
                    table.ForeignKey(
                        name: "FK_Zone_Farm_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "FarmId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zone_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Zone_User_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZoneUser",
                columns: table => new
                {
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneUser", x => new { x.ZoneId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ZoneUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ZoneUser_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityActivityLog_UserId",
                table: "EntityActivityLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Farm_OwnerUserId",
                table: "Farm",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Zone_CreatedByUserId",
                table: "Zone",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Zone_FarmId",
                table: "Zone",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_Zone_SupervisorId",
                table: "Zone",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneUser_UserId",
                table: "ZoneUser",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityActivityLog");

            migrationBuilder.DropTable(
                name: "ZoneUser");

            migrationBuilder.DropTable(
                name: "Zone");

            migrationBuilder.DropTable(
                name: "Farm");
        }
    }
}
