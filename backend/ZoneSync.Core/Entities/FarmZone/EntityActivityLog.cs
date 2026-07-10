using System;
using ZoneSync.Core.Enums;
using ZoneSync.Core.Entities.Identity;

namespace ZoneSync.Core.Entities.FarmZone
{
    public class EntityActivityLog
    {
        public int ActivityId { get; set; }

        public int UserId { get; set; }
        public UserProfile User { get; set; } = null!;

        public ActivityEntityType EntityType { get; set; }

        public int EntityId { get; set; }

        public ActivityActionType ActionType { get; set; }

        public DateTime ActionAt { get; set; }
    }
}