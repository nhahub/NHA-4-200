using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZoneSync.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertsTasksModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alert",
                columns: table => new
                {
                    AlertId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    CropPlanId = table.Column<int>(type: "int", nullable: true),
                    CheckId = table.Column<int>(type: "int", nullable: true),
                    RequirementId = table.Column<int>(type: "int", nullable: true),
                    SensorInstanceId = table.Column<int>(type: "int", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    ConfirmedByUserId = table.Column<int>(type: "int", nullable: true),
                    AlertType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FiringDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    AlertSeverity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AlertStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alert", x => x.AlertId);
                    table.ForeignKey(
                        name: "FK_Alert_CheckRequirements_CheckId",
                        column: x => x.CheckId,
                        principalTable: "CheckRequirements",
                        principalColumn: "CheckId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alert_CropPlans_CropPlanId",
                        column: x => x.CropPlanId,
                        principalTable: "CropPlans",
                        principalColumn: "CropPlanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alert_SensorInstances_SensorInstanceId",
                        column: x => x.SensorInstanceId,
                        principalTable: "SensorInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alert_StageRequirements_RequirementId",
                        column: x => x.RequirementId,
                        principalTable: "StageRequirements",
                        principalColumn: "ReqId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alert_User_ConfirmedByUserId",
                        column: x => x.ConfirmedByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alert_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alert_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    CropPlanId = table.Column<int>(type: "int", nullable: true),
                    AlertId = table.Column<int>(type: "int", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaskDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    TaskStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CompletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TaskPriority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActualVerificationAfterHours = table.Column<int>(type: "int", nullable: true),
                    TaskType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.TaskId);
                    table.CheckConstraint("CHK_Task_AlertConsistency", "(TaskType = 'BasedOnAlert' AND AlertId IS NOT NULL) OR (TaskType = 'Manual' AND AlertId IS NULL)");
                    table.ForeignKey(
                        name: "FK_Task_Alert_AlertId",
                        column: x => x.AlertId,
                        principalTable: "Alert",
                        principalColumn: "AlertId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Task_CropPlans_CropPlanId",
                        column: x => x.CropPlanId,
                        principalTable: "CropPlans",
                        principalColumn: "CropPlanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Task_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Task_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActionLog",
                columns: table => new
                {
                    ActionLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    ExecutedByUserId = table.Column<int>(type: "int", nullable: false),
                    QuantityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QuantityDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionLog", x => x.ActionLogId);
                    table.ForeignKey(
                        name: "FK_ActionLog_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActionLog_User_ExecutedByUserId",
                        column: x => x.ExecutedByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskUser",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskUser", x => new { x.TaskId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TaskUser_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_ExecutedByUserId",
                table: "ActionLog",
                column: "ExecutedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_TaskId",
                table: "ActionLog",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alert_CheckId",
                table: "Alert",
                column: "CheckId");

            migrationBuilder.CreateIndex(
                name: "IX_Alert_ConfirmedByUserId",
                table: "Alert",
                column: "ConfirmedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Alert_CreatedByUserId",
                table: "Alert",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Alert_CropPlanId",
                table: "Alert",
                column: "CropPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Alert_RequirementId",
                table: "Alert",
                column: "RequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_Alert_SensorInstanceId",
                table: "Alert",
                column: "SensorInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Alert_ZoneId",
                table: "Alert",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_AlertId",
                table: "Task",
                column: "AlertId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_CreatedByUserId",
                table: "Task",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_CropPlanId",
                table: "Task",
                column: "CropPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_ZoneId",
                table: "Task",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskUser_UserId",
                table: "TaskUser",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionLog");

            migrationBuilder.DropTable(
                name: "TaskUser");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "Alert");
        }
    }
}
