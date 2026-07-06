using System;
using System.Threading.Tasks;
using ZoneSync.Core.Entities;
using ZoneSync.Core.Entities.CropPlanModule;

namespace ZoneSync.Service.Contracts
{
    public interface ICropPlanService
    {
        Task<CropPlan> CreateCropPlanAsync(int cropId, int zoneId, DateTime plantingDate);
        void CalculateIsSatisfied(CheckRequirement checkReq);
    }
}