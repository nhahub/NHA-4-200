namespace ZoneSync.Web.Models
{
    public class FarmDashboardViewModel
    {
        public int FarmId { get; set; }
        public string FarmName { get; set; } = string.Empty;
        public string FarmLocation { get; set; } = string.Empty;
        public string SoilType { get; set; } = string.Empty;
        public decimal TotalArea { get; set; }
        public int ActiveZonesCount { get; set; }
        public int TotalAlertsCount { get; set; }
        public int UnderReviewAlertsCount { get; set; }
        public int ConfirmedAlertsCount { get; set; }
        public int UrgentTasksCount { get; set; }
        public int PendingTasksCount { get; set; }
        public int InProgressTasksCount { get; set; }
        public List<ZoneSummaryItem> Zones { get; set; } = new();
        public List<AlertSummaryItem> RecentAlerts { get; set; } = new();
        public List<TaskSummaryItem> UrgentTasks { get; set; } = new();
        public List<ReadingSummaryItem> AverageReadings { get; set; } = new();
    }

    public class ZoneSummaryItem
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public decimal ZoneArea { get; set; }
        public string ZoneStatus { get; set; } = string.Empty;
        public int OpenAlertsCount { get; set; }
        public int OpenTasksCount { get; set; }
        public string? SupervisorName { get; set; }
    }

    public class AlertSummaryItem
    {
        public int AlertId { get; set; }
        public string AlertType { get; set; } = string.Empty;
        public string AlertSeverity { get; set; } = string.Empty;
        public string AlertStatus { get; set; } = string.Empty;
        public DateTime FiringDate { get; set; }
        public string ZoneName { get; set; } = string.Empty;
    }

    public class TaskSummaryItem
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string TaskStatus { get; set; } = string.Empty;
        public string TaskPriority { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public string ZoneName { get; set; } = string.Empty;
    }

    public class ReadingSummaryItem
    {
        public string MeasurementName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal AverageValue { get; set; }
    }

    public class ZoneDashboardViewModel
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public decimal ZoneArea { get; set; }
        public string ZoneStatus { get; set; } = string.Empty;
        public string FarmName { get; set; } = string.Empty;
        public int FarmId { get; set; }
        public string? SupervisorName { get; set; }
        public int AssignedUsersCount { get; set; }

        public CurrentCropPlanInfo? CurrentCropPlan { get; set; }
        public List<SensorStatusInfo> SensorStatuses { get; set; } = new();
        public List<StageReadingInfo> StageReadings { get; set; } = new();
        public List<AlertSummaryItem> OpenAlerts { get; set; } = new();
        public List<TaskSummaryItem> OpenTasks { get; set; } = new();
    }

    public class CurrentCropPlanInfo
    {
        public int CropPlanId { get; set; }
        public string CropName { get; set; } = string.Empty;
        public DateTime PlantingDate { get; set; }
        public DateTime PredictedHarvestTime { get; set; }
        public DateTime? ActualHarvestTime { get; set; }
        public string CurrentStageName { get; set; } = string.Empty;
        public int CurrentStageOrder { get; set; }
        public int TotalStages { get; set; }
        public string CurrentStageStatus { get; set; } = string.Empty;
        public DateTime? StagePredictedEndDate { get; set; }
    }

    public class SensorStatusInfo
    {
        public int SensorInstanceId { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? LastReadingDate { get; set; }
        public decimal? LastReadingValue { get; set; }
        public string LastReadingType { get; set; } = string.Empty;
    }

    public class StageReadingInfo
    {
        public string RequirementName { get; set; } = string.Empty;
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal? LatestReadingValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool? IsSatisfied { get; set; }
    }
}
