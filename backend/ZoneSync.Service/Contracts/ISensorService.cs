using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZoneSync.Core.Entities.Sensors;

namespace ZoneSync.Service.Contracts
{
    public interface ISensorService
    {
        Task<SensorModel> CreateSensorModelAsync(SensorModel sensorModel, IEnumerable<int> measurementTypeIds);
        Task UpdateSensorModelMeasurementTypesAsync(int sensorModelId, IEnumerable<int> measurementTypeIds);
        Task<ZoneConfiguration> ConfigureSensorInZoneAsync(int zoneId, int sensorInstanceId, int configuredByUserId, DateTime configuredAt);
        Task<SensorReading> AddReadingAsync(int sensorInstanceId, int measurementTypeId, decimal readingValue, DateTime readingDate, TimeSpan readingTime);
    }
}
