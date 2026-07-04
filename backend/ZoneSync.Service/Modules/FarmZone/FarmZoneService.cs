using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities.FarmZone;
using ZoneSync.Core.Enums;
using ZoneSync.Service.Contracts;

namespace ZoneSync.Service.Modules.FarmZone
{
    public class FarmZoneService : IFarmZoneService
    {
        private readonly ApplicationDbContext context;
        private readonly IIdentityService identityService;

        public FarmZoneService(ApplicationDbContext context, IIdentityService identityService)
        {
            this.context = context;
            this.identityService = identityService;
        }

        // ---------------------------------------------------------------
        // FARM
        // ---------------------------------------------------------------

        public async Task<Core.Entities.FarmZone.Farm> CreateFarmAsync(string farmName, string farmLocation, string soilType, int ownerUserId)
        {
            var farm = new Core.Entities.FarmZone.Farm
            {
                FarmName = farmName,
                FarmLocation = farmLocation,
                SoilType = soilType,
                OwnerUserId = ownerUserId,
                TotalArea = 0,
                NoOfZones = 0,
                IsDeleted = false
            };

            context.Farms.Add(farm);
            await context.SaveChangesAsync(); // save first so farm.FarmId is generated

            // Owner gets a FarmMembership row automatically — reuses Member 1's
            // AddFarmMembershipAsync (same method AcceptInvitationAsync calls for
            // invited Engineers/Farmers). That method does NOT call SaveChangesAsync
            // itself, so it's done here explicitly.
            await identityService.AddFarmMembershipAsync(farm.FarmId, ownerUserId, FarmRoleType.Owner);
            await context.SaveChangesAsync();

            return farm;
        }

        public async Task UpdateFarmAsync(int farmId, string farmName, string farmLocation, string soilType)
        {
            var farm = await context.Farms.FindAsync(farmId);
            if (farm == null || farm.IsDeleted)
                throw new InvalidOperationException("Farm not found.");

            farm.FarmName = farmName;
            farm.FarmLocation = farmLocation;
            farm.SoilType = soilType;

            await context.SaveChangesAsync();
        }

        public async Task SoftDeleteFarmAsync(int farmId)
        {
            var farm = await context.Farms.FindAsync(farmId);
            if (farm == null || farm.IsDeleted)
                throw new InvalidOperationException("Farm not found.");

            farm.IsDeleted = true;
            farm.SoftDeletedAt = DateTime.Now;

            await context.SaveChangesAsync();

            // NOTE (agreed earlier): soft-deleting a Farm does NOT cascade-delete its
            // Zones. Zones just get filtered out wherever the app queries "zones for
            // this farm", by checking Farm.IsDeleted along the way.
        }

        public async Task<Core.Entities.FarmZone.Farm?> GetFarmAsync(int farmId)
        {
            return await context.Farms.FirstOrDefaultAsync(f => f.FarmId == farmId && !f.IsDeleted);
        }

        public async Task<System.Collections.Generic.List<Zone>> GetActiveZonesForFarmAsync(int farmId)
        {
            // Per the task doc: Farm Details must only show non-deleted zones.
            return await context.Zones
                .Where(z => z.FarmId == farmId && !z.IsDeleted)
                .ToListAsync();
        }

        public async Task<Zone?> GetZoneAsync(int zoneId)
        {
            return await context.Zones
                .Include(z => z.Farm)
                .Include(z => z.Supervisor)
                .Include(z => z.ZoneUsers)
                    .ThenInclude(zu => zu.User)
                .FirstOrDefaultAsync(z => z.ZoneId == zoneId && !z.IsDeleted);
        }

        public async Task<System.Collections.Generic.List<(int UserId, string FullName, FarmRoleType RoleType)>> GetFarmMembersAsync(int farmId)
        {
            var members = await context.FarmMemberships
                .Include(fm => fm.UserProfile)
                .Where(fm => fm.FarmId == farmId)
                .ToListAsync();

            return members
                .Select(m => (m.UserProfile.UserId, $"{m.UserProfile.UserFirstName} {m.UserProfile.UserLastName}", m.RoleType))
                .ToList();
        }

        // ---------------------------------------------------------------
        // ZONE
        // ---------------------------------------------------------------

        public async Task<Zone> CreateZoneAsync(int farmId, string zoneName, decimal zoneArea, int createdByUserId)
        {
            var farm = await context.Farms.FindAsync(farmId);
            if (farm == null || farm.IsDeleted)
                throw new InvalidOperationException("Farm not found.");

            var zone = new Zone
            {
                FarmId = farmId,
                ZoneName = zoneName,
                ZoneArea = zoneArea,
                ZoneStatus = ZoneStatus.Available,
                CreatedByUserId = createdByUserId,
                IsDeleted = false
            };

            context.Zones.Add(zone);
            await context.SaveChangesAsync();

            await RecalculateFarmTotalsAsync(farmId);
            await WriteActivityLogAsync(createdByUserId, ActivityEntityType.Zone, zone.ZoneId, ActivityActionType.Create);

            return zone;
        }

        public async Task UpdateZoneAsync(int zoneId, string zoneName, decimal zoneArea, ZoneStatus status)
        {
            var zone = await context.Zones.FindAsync(zoneId);
            if (zone == null || zone.IsDeleted)
                throw new InvalidOperationException("Zone not found.");

            zone.ZoneName = zoneName;
            zone.ZoneArea = zoneArea;
            zone.ZoneStatus = status;

            await context.SaveChangesAsync();

            await RecalculateFarmTotalsAsync(zone.FarmId);
            await WriteActivityLogAsync(zone.CreatedByUserId, ActivityEntityType.Zone, zone.ZoneId, ActivityActionType.Update);
        }

        public async Task SoftDeleteZoneAsync(int zoneId)
        {
            var zone = await context.Zones.FindAsync(zoneId);
            if (zone == null || zone.IsDeleted)
                throw new InvalidOperationException("Zone not found.");

            zone.IsDeleted = true;
            zone.SoftDeletedAt = DateTime.Now;

            await context.SaveChangesAsync();

            await RecalculateFarmTotalsAsync(zone.FarmId);
            await WriteActivityLogAsync(zone.CreatedByUserId, ActivityEntityType.Zone, zone.ZoneId, ActivityActionType.Delete);

            // NOTE (agreed earlier): ZoneUser rows for this zone are NOT deleted here.
            // They're left in place and simply filtered out anywhere the app queries
            // "who's assigned to this zone" by checking Zone.IsDeleted first.
        }

        // ---------------------------------------------------------------
        // ZONE ASSIGNMENTS
        // ---------------------------------------------------------------

        public async Task AssignUserToZoneAsync(int zoneId, int userId)
        {
            var zone = await context.Zones.FindAsync(zoneId);
            if (zone == null || zone.IsDeleted)
                throw new InvalidOperationException("Zone not found.");

            bool alreadyAssigned = await context.ZoneUsers
                .AnyAsync(zu => zu.ZoneId == zoneId && zu.UserId == userId);

            if (alreadyAssigned)
                return;

            context.ZoneUsers.Add(new Core.Entities.FarmZone.ZoneUser
            {
                ZoneId = zoneId,
                UserId = userId
            });

            await context.SaveChangesAsync();
        }

        public async Task SetZoneSupervisorAsync(int zoneId, int supervisorUserId)
        {
            var zone = await context.Zones.FindAsync(zoneId);
            if (zone == null || zone.IsDeleted)
                throw new InvalidOperationException("Zone not found.");

            bool isAssignedToZone = await context.ZoneUsers
                .AnyAsync(zu => zu.ZoneId == zoneId && zu.UserId == supervisorUserId);

            if (!isAssignedToZone)
                throw new InvalidOperationException(
                    "This user must be assigned to the zone before they can be made supervisor.");

            // Confirms the candidate holds the Engineer role on this zone's farm,
            // via Member 1's FarmMembership table. Uses the FarmRoleType enum
            // directly — not a string comparison — matching how RoleType is
            // actually stored (see IdentityService.AddFarmMembershipAsync).
            bool isEngineerOnFarm = await context.FarmMemberships
                .AnyAsync(fm =>
                    fm.FarmId == zone.FarmId &&
                    fm.UserId == supervisorUserId &&
                    fm.RoleType == FarmRoleType.Engineer);

            if (!isEngineerOnFarm)
                throw new InvalidOperationException(
                    "This user must hold the Engineer role on this farm to be made supervisor.");

            zone.SupervisorId = supervisorUserId;
            await context.SaveChangesAsync();
        }

        // ---------------------------------------------------------------
        // INTERNAL HELPERS
        // ---------------------------------------------------------------

        private async Task RecalculateFarmTotalsAsync(int farmId)
        {
            var farm = await context.Farms.FindAsync(farmId);
            if (farm == null) return;

            var activeZones = context.Zones.Where(z => z.FarmId == farmId && !z.IsDeleted);

            farm.NoOfZones = await activeZones.CountAsync();
            farm.TotalArea = await activeZones.SumAsync(z => (decimal?)z.ZoneArea) ?? 0;

            await context.SaveChangesAsync();
        }

        private async Task WriteActivityLogAsync(
            int userId,
            ActivityEntityType entityType,
            int entityId,
            ActivityActionType actionType)
        {
            context.EntityActivityLogs.Add(new Core.Entities.FarmZone.EntityActivityLog
            {
                UserId = userId,
                EntityType = entityType,
                EntityId = entityId,
                ActionType = actionType,
                ActionAt = DateTime.Now
            });

            await context.SaveChangesAsync();
        }
    }
}