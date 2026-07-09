using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities.AlertsTasks;
using ZoneSync.Core.Enums;
using ZoneSync.Service.Contracts;

namespace ZoneSync.Service.Modules.AlertsTasks
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        // تحويل خطورة الـ Alert لأولوية Task (Critical مفيش لها مقابل في TaskPriority فهنعتبرها High)
        private static TaskPriority MapSeverityToPriority(AlertSeverity severity) => severity switch
        {
            AlertSeverity.Critical => TaskPriority.High,
            AlertSeverity.High => TaskPriority.High,
            AlertSeverity.Medium => TaskPriority.Medium,
            _ => TaskPriority.Low
        };

        public async Task<TaskItem> CreateFromAlertAsync(int alertId, string taskName, string? taskDesc,
            DateOnly dueDate, int createdByUserId, int? actualVerificationAfterHours = null)
        {
            var alert = await _context.Alerts
                .Include(a => a.StageRequirement)
                .FirstOrDefaultAsync(a => a.AlertId == alertId)
                ?? throw new KeyNotFoundException("Alert not found");

            // لو مفيش قيمة متبعتة، خدي الـ default من StageRequirement (زي ما موضح في SQL)
            var verificationHours = actualVerificationAfterHours
                ?? alert.StageRequirement?.DefaultVerificationAfterHours;

            var task = new TaskItem
            {
                ZoneId = alert.ZoneId,
                CropPlanId = alert.CropPlanId,
                AlertId = alert.AlertId,
                CreatedByUserId = createdByUserId,
                TaskName = taskName,
                TaskDesc = taskDesc,
                CreatedAt = DateTime.Now,
                TaskStatus = Core.Enums.TaskStatus.Pending,
                DueDate = dueDate,
                TaskPriority = MapSeverityToPriority(alert.AlertSeverity),
                ActualVerificationAfterHours = verificationHours,
                TaskType = TaskType.BasedOnAlert
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem> CreateManualAsync(int zoneId, int? cropPlanId, string taskName,
            string? taskDesc, DateOnly dueDate, TaskPriority priority, int createdByUserId)
        {
            var task = new TaskItem
            {
                ZoneId = zoneId,
                CropPlanId = cropPlanId,
                AlertId = null,
                CreatedByUserId = createdByUserId,
                TaskName = taskName,
                TaskDesc = taskDesc,
                CreatedAt = DateTime.Now,
                TaskStatus = Core.Enums.TaskStatus.Pending,
                DueDate = dueDate,
                TaskPriority = priority,
                TaskType = TaskType.Manual
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem?> GetByIdWithDetailsAsync(int taskId)
        {
            return await _context.Tasks
                .Include(t => t.Zone)
                .Include(t => t.CropPlan)
                .Include(t => t.Alert)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedUsers).ThenInclude(au => au.User)
                .Include(t => t.ActionLog)
                .FirstOrDefaultAsync(t => t.TaskId == taskId);
        }

        public async Task<List<TaskItem>> GetByZoneAsync(int zoneId)
        {
            return await _context.Tasks
                .Where(t => t.ZoneId == zoneId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task AssignUserToTaskAsync(int taskId, int userId)
        {
            var exists = await _context.TaskUsers
                .AnyAsync(tu => tu.TaskId == taskId && tu.UserId == userId);

            if (exists) return; // متسندهاش تاني لنفس اليوزر

            _context.TaskUsers.Add(new TaskUser
            {
                TaskId = taskId,
                UserId = userId,
                AssignedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        public async Task StartTaskAsync(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId)
                ?? throw new KeyNotFoundException("Task not found");

            task.TaskStatus = Core.Enums.TaskStatus.InProgress;
            await _context.SaveChangesAsync();
        }

        public async Task<ActionLog> AddActionLogAsync(int taskId, int executedByUserId, string? quantityType,
            string? quantityDesc, string? result, string? notes)
        {
            var alreadyLogged = await _context.ActionLogs.AnyAsync(al => al.TaskId == taskId);
            if (alreadyLogged)
                throw new InvalidOperationException("Task already has an Action Log (1:1 relationship)");

            var log = new ActionLog
            {
                TaskId = taskId,
                ExecutedByUserId = executedByUserId,
                QuantityType = quantityType,
                QuantityDesc = quantityDesc,
                ExecutedAt = DateTime.Now,
                Result = result,
                Notes = notes
            };

            _context.ActionLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task CompleteTaskAsync(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId)
                ?? throw new KeyNotFoundException("Task not found");

            task.TaskStatus = Core.Enums.TaskStatus.Completed;
            task.CompletionTime = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task FailTaskAsync(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId)
                ?? throw new KeyNotFoundException("Task not found");

            task.TaskStatus = Core.Enums.TaskStatus.Failed;
            task.CompletionTime = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task SkipTaskAsync(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId)
                ?? throw new KeyNotFoundException("Task not found");

            task.TaskStatus = Core.Enums.TaskStatus.Skipped;
            await _context.SaveChangesAsync();
        }
    }
}