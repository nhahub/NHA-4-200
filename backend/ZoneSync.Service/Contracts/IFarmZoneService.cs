using System;
using System.Threading.Tasks;
using ZoneSync.Core.Enums;
using ZoneSync.Core.Entities.FarmZone;

namespace ZoneSync.Service.Contracts
{
    public interface IFarmZoneService
    {
        Task<Farm> CreateFarmAsync(string farmName, string farmLocation, string soilType, int ownerUserId);
        Task UpdateFarmAsync(int farmId, string farmName, string farmLocation, string soilType);
        Task SoftDeleteFarmAsync(int farmId);

        Task<Zone> CreateZoneAsync(int farmId, string zoneName, decimal zoneArea, int createdByUserId);
        Task UpdateZoneAsync(int zoneId, string zoneName, decimal zoneArea,ZoneStatus status);
        Task SoftDeleteZoneAsync(int zoneId);

        Task AssignUserToZoneAsync(int zoneId, int userId);
        Task SetZoneSupervisorAsync(int zoneId, int supervisorUserId);
    }
}