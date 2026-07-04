using System.Threading.Tasks;
using System.Collections.Generic;
using ZoneSync.Core.Entities.FarmZone;
using ZoneSync.Core.Enums;

namespace ZoneSync.Service.Contracts
{
    public interface IFarmZoneService
    {
        Task<Farm> CreateFarmAsync(string farmName, string farmLocation, string soilType, int ownerUserId);
        Task UpdateFarmAsync(int farmId, string farmName, string farmLocation, string soilType);
        Task SoftDeleteFarmAsync(int farmId);
        Task<Farm?> GetFarmAsync(int farmId);
        Task<List<Zone>> GetActiveZonesForFarmAsync(int farmId);

        Task<Zone> CreateZoneAsync(int farmId, string zoneName, decimal zoneArea, int createdByUserId);
        Task UpdateZoneAsync(int zoneId, string zoneName, decimal zoneArea, ZoneStatus status);
        Task SoftDeleteZoneAsync(int zoneId);
        Task<Zone?> GetZoneAsync(int zoneId);

        Task AssignUserToZoneAsync(int zoneId, int userId);
        Task SetZoneSupervisorAsync(int zoneId, int supervisorUserId);

        // Used to populate the "Assigned Users" / "Supervisor" pickers on the
        // Zone Create/Details views — returns members of the zone's farm
        // (via FarmMembership), not every user in the system.
        Task<List<(int UserId, string FullName, FarmRoleType RoleType)>> GetFarmMembersAsync(int farmId);
    }
}