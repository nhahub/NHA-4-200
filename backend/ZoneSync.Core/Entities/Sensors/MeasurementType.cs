using System.Collections.Generic;

namespace ZoneSync.Core.Entities.Sensors
{
    public class MeasurementType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        public ICollection<SensorModelMeasurementType> SensorModelMeasurementTypes { get; set; } = new List<SensorModelMeasurementType>();
        public ICollection<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();
    }
}
