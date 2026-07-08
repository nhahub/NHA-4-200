using System;

namespace ZoneSync.Core.Entities.Sensors
{
    public class SensorReading
    {
        public int Id { get; set; }
        public int SensorInstanceId { get; set; }
        public int MeasurementTypeId { get; set; }
        public decimal ReadingValue { get; set; }
        public DateTime ReadingDate { get; set; } = DateTime.Today;
        public TimeSpan ReadingTime { get; set; } = DateTime.Now.TimeOfDay;

        public SensorInstance? SensorInstance { get; set; }
        public MeasurementType? MeasurementType { get; set; }
    }
}
