using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities.Sensors;
using ZoneSync.Core.Enums;
using ZoneSync.Service.Contracts;

namespace ZoneSync.Service.Modules.Sensors
{
    public class SensorService : ISensorService
    {
        private readonly ApplicationDbContext _context;

        public SensorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SensorModel> CreateSensorModelAsync(SensorModel sensorModel, IEnumerable<int> measurementTypeIds)
        {
            foreach (var measurementTypeId in measurementTypeIds.Distinct())
            {
                sensorModel.SensorModelMeasurementTypes.Add(new SensorModelMeasurementType
                {
                    MeasurementTypeId = measurementTypeId
                });
            }

            _context.SensorModels.Add(sensorModel);
            await _context.SaveChangesAsync();
            return sensorModel;
        }

        public async Task UpdateSensorModelMeasurementTypesAsync(int sensorModelId, IEnumerable<int> measurementTypeIds)
        {
            var existingLinks = await _context.SensorModelMeasurementTypes
                .Where(link => link.SensorModelId == sensorModelId)
                .ToListAsync();

            _context.SensorModelMeasurementTypes.RemoveRange(existingLinks);

            foreach (var measurementTypeId in measurementTypeIds.Distinct())
            {
                _context.SensorModelMeasurementTypes.Add(new SensorModelMeasurementType
                {
                    SensorModelId = sensorModelId,
                    MeasurementTypeId = measurementTypeId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<ZoneConfiguration> ConfigureSensorInZoneAsync(int zoneId, int sensorInstanceId, int configuredByUserId, DateTime configuredAt)
        {
            var configuration = new ZoneConfiguration
            {
                ZoneId = zoneId,
                SensorInstanceId = sensorInstanceId,
                ConfiguredByUserId = configuredByUserId,
                ConfiguredAt = configuredAt
            };

            _context.ZoneConfigurations.Add(configuration);

            var sensorInstance = await _context.SensorInstances.FindAsync(sensorInstanceId);
            if (sensorInstance is not null)
            {
                sensorInstance.Status = SensorStatus.Configured;
            }

            await _context.SaveChangesAsync();
            return configuration;
        }

        public async Task<SensorReading> AddReadingAsync(int sensorInstanceId, int measurementTypeId, decimal readingValue, DateTime readingDate, TimeSpan readingTime)
        {
            var reading = new SensorReading
            {
                SensorInstanceId = sensorInstanceId,
                MeasurementTypeId = measurementTypeId,
                ReadingValue = readingValue,
                ReadingDate = readingDate,
                ReadingTime = readingTime
            };

            _context.SensorReadings.Add(reading);
            await _context.SaveChangesAsync();
            return reading;
        }
    }
}
