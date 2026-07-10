using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Data;
using ZoneSync.Core.Enums;
using ZoneSync.Service.Contracts;
using ZoneSync.Web.Models;
// Type alias to resolve ambiguity with System.Threading.Tasks.TaskStatus
using STS = ZoneSync.Core.Enums.TaskStatus;

namespace ZoneSync.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFarmZoneService _farmZoneService;
        private readonly IAlertService _alertService;
        private readonly ITaskService _taskService;

        public DashboardController(
            ApplicationDbContext context,
            IFarmZoneService farmZoneService,
            IAlertService alertService,
            ITaskService taskService)
        {
            _context = context;
            _farmZoneService = farmZoneService;
            _alertService = alertService;
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> FarmDashboard(int farmId)
        {
            var farm = await _farmZoneService.GetFarmAsync(farmId);
            if (farm == null) return NotFound();

            var zones = await _context.Zones
                .Where(z => z.FarmId == farmId && !z.IsDeleted)
                .ToListAsync();

            var zoneIds = zones.Select(z => z.ZoneId).ToList();

            var alerts = await _context.Alerts
                .Where(a => zoneIds.Contains(a.ZoneId))
                .OrderByDescending(a => a.FiringDate)
                .Take(10)
                .ToListAsync();

            var tasks = await _context.Tasks
                .Where(t => zoneIds.Contains(t.ZoneId))
                .OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .ToListAsync();

            var zoneConfigurations = await _context.ZoneConfigurations
                .Where(zc => zoneIds.Contains(zc.ZoneId))
                .Include(zc => zc.SensorInstance)
                    .ThenInclude(si => si!.SensorReadings)
                .Include(zc => zc.SensorInstance!.SensorModel)
                    .ThenInclude(sm => sm!.SensorModelMeasurementTypes)
                    .ThenInclude(l => l.MeasurementType)
                .ToListAsync();

            var sensorInstanceIds = zoneConfigurations
                .Select(zc => zc.SensorInstanceId)
                .Distinct()
                .ToList();

            var latestReadings = await _context.SensorReadings
                .Where(sr => sensorInstanceIds.Contains(sr.SensorInstanceId))
                .Include(sr => sr.MeasurementType)
                .GroupBy(sr => sr.MeasurementTypeId)
                .Select(g => new
                {
                    MeasurementTypeId = g.Key,
                    MeasurementName = g.First().MeasurementType!.Name,
                    Unit = g.First().MeasurementType!.Unit,
                    AverageValue = g.Average(r => (double)r.ReadingValue)
                })
                .ToListAsync();

            var zoneSummaries = new List<ZoneSummaryItem>();
            foreach (var zone in zones)
            {
                zoneSummaries.Add(new ZoneSummaryItem
                {
                    ZoneId = zone.ZoneId,
                    ZoneName = zone.ZoneName,
                    ZoneArea = zone.ZoneArea,
                    ZoneStatus = zone.ZoneStatus.ToString(),
                    OpenAlertsCount = alerts.Count(a => a.ZoneId == zone.ZoneId && a.AlertStatus == AlertStatus.UnderReview),
                    OpenTasksCount = tasks.Count(t => t.ZoneId == zone.ZoneId && (t.TaskStatus == STS.Pending || t.TaskStatus == STS.InProgress)),
                    SupervisorName = zone.SupervisorId.HasValue
                        ? await _context.UserProfiles.Where(u => u.UserId == zone.SupervisorId.Value).Select(u => u.UserFirstName + " " + u.UserLastName).FirstOrDefaultAsync()
                        : null
                });
            }

            var model = new FarmDashboardViewModel
            {
                FarmId = farm.FarmId,
                FarmName = farm.FarmName,
                FarmLocation = farm.FarmLocation,
                SoilType = farm.SoilType,
                TotalArea = farm.TotalArea,
                ActiveZonesCount = zones.Count,
                TotalAlertsCount = alerts.Count,
                UnderReviewAlertsCount = alerts.Count(a => a.AlertStatus == AlertStatus.UnderReview),
                ConfirmedAlertsCount = alerts.Count(a => a.AlertStatus == AlertStatus.Confirmed),
                UrgentTasksCount = tasks.Count(t => t.TaskPriority == TaskPriority.High && t.TaskStatus != STS.Completed && t.TaskStatus != STS.Failed && t.TaskStatus != STS.Skipped),
                PendingTasksCount = tasks.Count(t => t.TaskStatus == STS.Pending),
                InProgressTasksCount = tasks.Count(t => t.TaskStatus == STS.InProgress),
                Zones = zoneSummaries,
                RecentAlerts = alerts.Select(a => new AlertSummaryItem
                {
                    AlertId = a.AlertId,
                    AlertType = a.AlertType.ToString(),
                    AlertSeverity = a.AlertSeverity.ToString(),
                    AlertStatus = a.AlertStatus.ToString(),
                    FiringDate = a.FiringDate,
                    ZoneName = zones.Where(z => z.ZoneId == a.ZoneId).Select(z => z.ZoneName).FirstOrDefault() ?? ""
                }).ToList(),
                UrgentTasks = tasks.Where(t => t.TaskPriority == TaskPriority.High && t.TaskStatus != STS.Completed && t.TaskStatus != STS.Failed && t.TaskStatus != STS.Skipped)
                    .Select(t => new TaskSummaryItem
                    {
                        TaskId = t.TaskId,
                        TaskName = t.TaskName,
                        TaskStatus = t.TaskStatus.ToString(),
                        TaskPriority = t.TaskPriority.ToString(),
                        DueDate = t.DueDate,
                        ZoneName = zones.Where(z => z.ZoneId == t.ZoneId).Select(z => z.ZoneName).FirstOrDefault() ?? ""
                    }).ToList(),
                AverageReadings = latestReadings.Select(r => new ReadingSummaryItem
                {
                    MeasurementName = r.MeasurementName,
                    Unit = r.Unit,
                    AverageValue = (decimal)r.AverageValue
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ZoneDashboard(int zoneId)
        {
            var zone = await _farmZoneService.GetZoneAsync(zoneId);
            if (zone == null) return NotFound();

            var activeCropPlan = await _context.CropPlans
                .Include(cp => cp.Crop)
                .Include(cp => cp.StageInformations)
                    .ThenInclude(si => si.GrowthStage)
                .Where(cp => cp.ZoneId == zoneId)
                .OrderByDescending(cp => cp.PlantingDate)
                .FirstOrDefaultAsync();

            var configuredSensors = await _context.ZoneConfigurations
                .Where(zc => zc.ZoneId == zoneId)
                .Include(zc => zc.SensorInstance)
                    .ThenInclude(si => si!.SensorModel)
                .Include(zc => zc.SensorInstance!.SensorReadings.OrderByDescending(r => r.ReadingDate).ThenByDescending(r => r.ReadingTime).Take(1))
                    .ThenInclude(r => r.MeasurementType)
                .ToListAsync();

            var alerts = await _context.Alerts
                .Where(a => a.ZoneId == zoneId && a.AlertStatus == AlertStatus.UnderReview)
                .OrderByDescending(a => a.FiringDate)
                .ToListAsync();

            var tasks = await _context.Tasks
                .Where(t => t.ZoneId == zoneId && (t.TaskStatus == STS.Pending || t.TaskStatus == STS.InProgress))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var sensorStatuses = new List<SensorStatusInfo>();
            foreach (var config in configuredSensors)
            {
                var lastReading = config.SensorInstance?.SensorReadings.FirstOrDefault();
                sensorStatuses.Add(new SensorStatusInfo
                {
                    SensorInstanceId = config.SensorInstanceId,
                    SerialNumber = config.SensorInstance?.SerialNumber ?? "",
                    ModelName = config.SensorInstance?.SensorModel?.ModelName ?? "",
                    Status = config.SensorInstance?.Status.ToString() ?? "",
                    LastReadingDate = lastReading?.ReadingDate + lastReading?.ReadingTime,
                    LastReadingValue = lastReading?.ReadingValue,
                    LastReadingType = lastReading?.MeasurementType?.Name ?? ""
                });
            }

            CurrentCropPlanInfo? cropPlanInfo = null;
            var stageReadings = new List<StageReadingInfo>();

            if (activeCropPlan != null)
            {
                var currentStage = activeCropPlan.StageInformations
                    .Where(si => si.StageStatus == "Current" || si.StageStatus == "Ongoing")
                    .OrderBy(si => si.GrowthStage?.StageOrder)
                    .FirstOrDefault();

                if (currentStage == null)
                {
                    currentStage = activeCropPlan.StageInformations
                        .Where(si => si.StageStatus == "NotStarted" || si.StageStatus == "Pending")
                        .OrderBy(si => si.GrowthStage?.StageOrder)
                        .FirstOrDefault();
                }

                cropPlanInfo = new CurrentCropPlanInfo
                {
                    CropPlanId = activeCropPlan.CropPlanId,
                    CropName = activeCropPlan.Crop?.CropName ?? "",
                    PlantingDate = activeCropPlan.PlantingDate,
                    PredictedHarvestTime = activeCropPlan.PredictedHarvestTime,
                    ActualHarvestTime = activeCropPlan.ActualHarvestTime,
                    CurrentStageName = currentStage?.StageName ?? "",
                    CurrentStageOrder = currentStage?.GrowthStage?.StageOrder ?? 0,
                    TotalStages = activeCropPlan.StageInformations.Count,
                    CurrentStageStatus = currentStage?.StageStatus ?? "",
                    StagePredictedEndDate = currentStage?.PredictedEndDate
                };

                if (currentStage?.GrowthStage != null)
                {
                    var requirements = await _context.StageRequirements
                        .Where(sr => sr.StageId == currentStage.GrowthStage.StageId)
                        .ToListAsync();

                    foreach (var req in requirements)
                    {
                        var latestCheck = await _context.CheckRequirements
                            .Where(cr => cr.CropPlanId == activeCropPlan.CropPlanId && cr.RequirementId == req.ReqId)
                            .OrderByDescending(cr => cr.LastCheckDate)
                            .FirstOrDefaultAsync();

                        var measType = await _context.MeasurementTypes
                            .FirstOrDefaultAsync(mt => mt.Name == req.ReqName);

                        stageReadings.Add(new StageReadingInfo
                        {
                            RequirementName = req.ReqName,
                            MinValue = req.MinValue,
                            MaxValue = req.MaxValue,
                            LatestReadingValue = latestCheck?.CheckedValue,
                            Unit = measType?.Unit ?? "",
                            IsSatisfied = latestCheck?.IsSatisfied
                        });
                    }
                }
            }

            var model = new ZoneDashboardViewModel
            {
                ZoneId = zone.ZoneId,
                ZoneName = zone.ZoneName,
                ZoneArea = zone.ZoneArea,
                ZoneStatus = zone.ZoneStatus.ToString(),
                FarmName = zone.Farm?.FarmName ?? "",
                FarmId = zone.FarmId,
                SupervisorName = zone.Supervisor != null ? $"{zone.Supervisor.UserFirstName} {zone.Supervisor.UserLastName}" : null,
                AssignedUsersCount = zone.ZoneUsers?.Count ?? 0,
                CurrentCropPlan = cropPlanInfo,
                SensorStatuses = sensorStatuses,
                StageReadings = stageReadings,
                OpenAlerts = alerts.Select(a => new AlertSummaryItem
                {
                    AlertId = a.AlertId,
                    AlertType = a.AlertType.ToString(),
                    AlertSeverity = a.AlertSeverity.ToString(),
                    AlertStatus = a.AlertStatus.ToString(),
                    FiringDate = a.FiringDate,
                    ZoneName = zone.ZoneName
                }).ToList(),
                OpenTasks = tasks.Select(t => new TaskSummaryItem
                {
                    TaskId = t.TaskId,
                    TaskName = t.TaskName,
                    TaskStatus = t.TaskStatus.ToString(),
                    TaskPriority = t.TaskPriority.ToString(),
                    DueDate = t.DueDate,
                    ZoneName = zone.ZoneName
                }).ToList()
            };

            return View(model);
        }
    }
}
