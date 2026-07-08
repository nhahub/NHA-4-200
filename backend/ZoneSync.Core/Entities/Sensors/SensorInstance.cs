using System.Collections.Generic;
using ZoneSync.Core.Enums;

namespace ZoneSync.Core.Entities.Sensors
{
    public class SensorInstance
    {
        public int Id { get; set; }
        public int SensorModelId { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public SensorStatus Status { get; set; } = SensorStatus.Available;

        public SensorModel? SensorModel { get; set; }
        public ICollection<ZoneConfiguration> ZoneConfigurations { get; set; } = new List<ZoneConfiguration>();
        public ICollection<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();
    }
}
