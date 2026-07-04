using System;
using System.Collections.Generic;
using ZoneSync.Core.Enums;
using ZoneSync.Core.Entities.Identity;

namespace ZoneSync.Core.Entities.FarmZone
{
    public class Zone
    {
        public int ZoneId { get; set; }

        public string ZoneName { get; set; }
        public decimal ZoneArea { get; set; }

        public ZoneStatus ZoneStatus { get; set; }

        public int CreatedByUserId { get; set; }
        public UserProfile CreatedByUser { get; set; }

        public int? SupervisorId { get; set; }
        public UserProfile Supervisor { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? SoftDeletedAt { get; set; }

        public int FarmId { get; set; }
        public Farm Farm { get; set; }

        // Navigation: users (engineers/farmers) assigned to this zone.
        // Role (Engineer vs Farmer) is NOT stored here — derive it via:
        // ZoneUser -> Zone -> Farm -> FarmMembership(UserId, FarmId).RoleType
        public ICollection<ZoneUser> ZoneUsers { get; set; } = new List<ZoneUser>();
    }
}