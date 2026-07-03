using ZoneSync.Core.Enums;

namespace ZoneSync.Core.Entities.Identity
{
    public class FarmMembership
    {
        public int FarmId { get; set; }
        public int UserId { get; set; }
        public FarmRoleType RoleType { get; set; }
        public DateTime JoinedAt { get; set; }

        public UserProfile UserProfile { get; set; } = null!;
    }
}
