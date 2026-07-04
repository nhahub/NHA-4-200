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
            await context.SaveChangesAsync(); 

           
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

        }

        public async Task<Core.Entities.FarmZone.Farm?> GetFarmAsync(int farmId)
        {
            return await context.Farms.FirstOrDefaultAsync(f => f.FarmId == farmId && !f.IsDeleted);
        }

        public async Task<System.Collections.Generic.List<Zone>> GetActiveZonesForFarmAsync(int farmId)
        {   return await context.Zones
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