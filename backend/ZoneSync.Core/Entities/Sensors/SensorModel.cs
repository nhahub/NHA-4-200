using System.Collections.Generic;

namespace ZoneSync.Core.Entities.Sensors
{
    public class SensorModel
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string OutputType { get; set; } = string.Empty;

        public ICollection<SensorModelMeasurementType> SensorModelMeasurementTypes { get; set; } = new List<SensorModelMeasurementType>();
        public ICollection<SensorInstance> SensorInstances { get; set; } = new List<SensorInstance>();
    }
}
