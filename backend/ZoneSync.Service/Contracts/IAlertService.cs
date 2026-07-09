using ZoneSync.Core.Entities.AlertsTasks;
using ZoneSync.Core.Enums;

namespace ZoneSync.Service.Contracts
{
    public interface IAlertService
    {
        Task<Alert> CreateFromCheckAsync(int checkId, int zoneId, int? cropPlanId, int requirementId,
            int? sensorInstanceId, AlertType alertType, AlertSeverity severity, int createdByUserId);

        Task<Alert> CreateHardwareMissingAsync(int zoneId, int requirementId, int? sensorInstanceId,
            int createdByUserId);

        Task<Alert> CreateManualAsync(int zoneId, int? cropPlanId, AlertSeverity severity, int createdByUserId);

        Task<Alert?> GetByIdAsync(int alertId);

        Task<List<Alert>> GetByZoneAsync(int zoneId);

        Task ConfirmAsync(int alertId, int confirmedByUserId);

        Task DiscardAsync(int alertId, int confirmedByUserId);

        // منطق منع تكرار نفس التنبيه - راجعي وثيقة الـ Alert&Action
        Task<bool> ShouldFireAlertAsync(int zoneId, int requirementId, int verificationHours);
    }
}