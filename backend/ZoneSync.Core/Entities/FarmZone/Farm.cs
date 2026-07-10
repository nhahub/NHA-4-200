using System;
using System.Collections.Generic;
using System.Security.Policy;
using ZoneSync.Core.Entities.Identity;

namespace ZoneSync.Core.Entities.FarmZone
{
    public class Farm
    {
        public int FarmId { get; set; }

        public int OwnerUserId { get; set; }
        public UserProfile OwnerUser { get; set; } = null!;

        public required string FarmName { get; set; }
        public required string FarmLocation { get; set; } = null!;
        public required string SoilType { get; set; } = null!;

        public decimal TotalArea { get; set; }
        public int NoOfZones { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? SoftDeletedAt { get; set; }

        public ICollection<Zone> Zones { get; set; } = new List<Zone>();
    }
}