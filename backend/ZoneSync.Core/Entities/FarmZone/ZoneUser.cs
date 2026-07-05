using System.Security.Policy;
using ZoneSync.Core.Entities.Identity;

namespace ZoneSync.Core.Entities.FarmZone
{
    public class ZoneUser
    {
        public int ZoneId { get; set; }
        public Zone Zone { get; set; }

        public int UserId { get; set; }
        public UserProfile User { get; set; }
    }
}