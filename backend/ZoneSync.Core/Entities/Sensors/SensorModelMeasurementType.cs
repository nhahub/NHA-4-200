namespace ZoneSync.Core.Entities.Sensors
{
    public class SensorModelMeasurementType
    {
        public int SensorModelId { get; set; }
        public int MeasurementTypeId { get; set; }

        public SensorModel? SensorModel { get; set; }
        public MeasurementType? MeasurementType { get; set; }
    }
}
