using ZoneSync.Core.Entities.AlertsTasks;
using ZoneSync.Core.Enums;

namespace ZoneSync.Service.Contracts
{
    public interface ITaskService
    {
        Task<TaskItem> CreateFromAlertAsync(int alertId, string taskName, string? taskDesc,
            DateOnly dueDate, int createdByUserId, int? actualVerificationAfterHours = null);

        Task<TaskItem> CreateManualAsync(int zoneId, int? cropPlanId, string taskName, string? taskDesc,
            DateOnly dueDate, TaskPriority priority, int createdByUserId);

        Task<TaskItem?> GetByIdWithDetailsAsync(int taskId);

        Task<List<TaskItem>> GetByZoneAsync(int zoneId);

        Task AssignUserToTaskAsync(int taskId, int userId);

        Task StartTaskAsync(int taskId);

        Task<ActionLog> AddActionLogAsync(int taskId, int executedByUserId, string? quantityType,
            string? quantityDesc, string? result, string? notes);

        Task CompleteTaskAsync(int taskId);

        Task FailTaskAsync(int taskId);

        Task SkipTaskAsync(int taskId);
    }
}