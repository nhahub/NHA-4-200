using System;
using System.Collections.Generic;
using ZoneSync.Core.Enums;
using ZoneSync.Core.Entities.Identity;

namespace ZoneSync.Core.Entities.FarmZone
{
    public class Zone
    {
        public int ZoneId { get; set; }

        public required string ZoneName { get; set; }
        public decimal ZoneArea { get; set; }

        public ZoneStatus ZoneStatus { get; set; }

        public int CreatedByUserId { get; set; }
        public UserProfile CreatedByUser { get; set; } = null!;

        public int? SupervisorId { get; set; }
        public UserProfile Supervisor { get; set; } = null!;

        public bool IsDeleted { get; set; }
        public DateTime? SoftDeletedAt { get; set; }

        public int FarmId { get; set; }
        public Farm Farm { get; set; } = null!;

        public ICollection<ZoneUser> ZoneUsers { get; set; } = new List<ZoneUser>();
    }
}