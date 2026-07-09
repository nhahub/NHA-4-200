using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities.AlertsTasks;
using ZoneSync.Core.Enums;
using ZoneSync.Service.Contracts;

namespace ZoneSync.Service.Modules.AlertsTasks
{
    public class AlertService : IAlertService
    {
        private readonly ApplicationDbContext _context;

        public AlertService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Alert> CreateFromCheckAsync(int checkId, int zoneId, int? cropPlanId, int requirementId,
            int? sensorInstanceId, AlertType alertType, AlertSeverity severity, int createdByUserId)
        {
            var alert = new Alert
            {
                ZoneId = zoneId,
                CropPlanId = cropPlanId,
                CheckId = checkId,
                RequirementId = requirementId,
                SensorInstanceId = sensorInstanceId,
                CreatedByUserId = createdByUserId,
                AlertType = alertType,
                AlertSeverity = severity,
                AlertStatus = AlertStatus.UnderReview,
                FiringDate = DateTime.Now
            };

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();
            return alert;
        }

        public async Task<Alert> CreateHardwareMissingAsync(int zoneId, int requirementId, int? sensorInstanceId,
            int createdByUserId)
        {
            // ملحوظة: هنا CheckId بيفضل null لأنه أصلاً مفيش قراءة اتعملها Check عليها
            var alert = new Alert
            {
                ZoneId = zoneId,
                RequirementId = requirementId,
                SensorInstanceId = sensorInstanceId,
                CreatedByUserId = createdByUserId,
                AlertType = AlertType.HardwareMissing,
                AlertSeverity = AlertSeverity.Medium,
                AlertStatus = AlertStatus.UnderReview,
                FiringDate = DateTime.Now
            };

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();
            return alert;
        }

        public async Task<Alert> CreateManualAsync(int zoneId, int? cropPlanId, AlertSeverity severity,
            int createdByUserId)
        {
            var alert = new Alert
            {
                ZoneId = zoneId,
                CropPlanId = cropPlanId,
                CreatedByUserId = createdByUserId,
                AlertType = AlertType.Manual,
                AlertSeverity = severity,
                AlertStatus = AlertStatus.UnderReview,
                FiringDate = DateTime.Now
            };

            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();
            return alert;
        }

        public async Task<Alert?> GetByIdAsync(int alertId)
        {
            return await _context.Alerts
                .Include(a => a.Zone)
                .Include(a => a.CropPlan)
                .Include(a => a.CreatedByUser)
                .Include(a => a.ConfirmedByUser)
                .Include(a => a.SensorInstance)
                .Include(a => a.Tasks)
                .FirstOrDefaultAsync(a => a.AlertId == alertId);
        }

        public async Task<List<Alert>> GetByZoneAsync(int zoneId)
        {
            return await _context.Alerts
                .Where(a => a.ZoneId == zoneId)
                .OrderByDescending(a => a.FiringDate)
                .ToListAsync();
        }

        public async Task ConfirmAsync(int alertId, int confirmedByUserId)
        {
            var alert = await _context.Alerts.FindAsync(alertId)
                ?? throw new KeyNotFoundException("Alert not found");

            alert.AlertStatus = AlertStatus.Confirmed;
            alert.ConfirmedByUserId = confirmedByUserId;
            await _context.SaveChangesAsync();
        }

        public async Task DiscardAsync(int alertId, int confirmedByUserId)
        {
            var alert = await _context.Alerts.FindAsync(alertId)
                ?? throw new KeyNotFoundException("Alert not found");

            alert.AlertStatus = AlertStatus.Discarded;
            alert.ConfirmedByUserId = confirmedByUserId;
            await _context.SaveChangesAsync();
        }

        // مبني على الخوارزمية اللي في وثيقة Alert & Action:
        // بنمنع alert جديد لو آخر alert لنفس الـ zone+requirement لسه جوه فترة الـ verification
        // والـ Task المرتبطة بيه لسه مش Completed/Failed (يعني المشكلة لسه مش اتحلت رسمي)
        public async Task<bool> ShouldFireAlertAsync(int zoneId, int requirementId, int verificationHours)
        {
            var lastAlert = await _context.Alerts
                .Include(a => a.Tasks)
                .Where(a => a.ZoneId == zoneId && a.RequirementId == requirementId)
                .OrderByDescending(a => a.FiringDate)
                .FirstOrDefaultAsync();

            if (lastAlert is null) return true; // أول alert من نوعه، اسمحي بيه

            var hoursSinceLastAlert = (DateTime.Now - lastAlert.FiringDate).TotalHours;

            if (hoursSinceLastAlert < verificationHours)
            {
                var relatedTask = lastAlert.Tasks.FirstOrDefault();
                var isResolved = relatedTask != null &&
                    (relatedTask.TaskStatus == Core.Enums.TaskStatus.Completed ||
                     relatedTask.TaskStatus == Core.Enums.TaskStatus.Failed);

                if (!isResolved)
                    return false; // تجاهل الـ alert الجديد، المشكلة القديمة لسه مفتوحة
            }

            return true;
        }
    }
}