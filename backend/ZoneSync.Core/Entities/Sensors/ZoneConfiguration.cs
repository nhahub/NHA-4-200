using System;
using ZoneSync.Core.Entities.FarmZone;
using ZoneSync.Core.Entities.Identity;

namespace ZoneSync.Core.Entities.Sensors
{
    public class ZoneConfiguration
    {
        public int Id { get; set; }
        public int ZoneId { get; set; }
        public int SensorInstanceId { get; set; }
        public int ConfiguredByUserId { get; set; }
        public DateTime ConfiguredAt { get; set; } = DateTime.Now;

        public Zone? Zone { get; set; }
        public SensorInstance? SensorInstance { get; set; }
        public UserProfile? ConfiguredByUser { get; set; }
    }
}
