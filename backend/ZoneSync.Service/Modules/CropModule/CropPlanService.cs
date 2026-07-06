using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities;
using ZoneSync.Core.Entities.CropPlanModule;
using ZoneSync.Service.Contracts;

namespace ZoneSync.Service.Modules.CropModule
{
    public class CropPlanService : ICropPlanService
    {
        private readonly ApplicationDbContext _context;

        public CropPlanService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<CropPlan> CreateCropPlanAsync(int cropId, int zoneId, DateTime plantingDate)
        {
            var cropStages = await _context.GrowthStages
                                           .Where(g => g.CropId == cropId)
                                           .ToListAsync();

            int totalDurationDays = cropStages.Sum(g => g.StageDuration);

            var cropPlan = new CropPlan
            {
                CropId = cropId,
                ZoneId = zoneId,
                PlantingDate = plantingDate,
                PredictedHarvestTime = plantingDate.AddDays(totalDurationDays) 
            };

            _context.CropPlans.Add(cropPlan);
            await _context.SaveChangesAsync();
            return cropPlan;
        }

        public void CalculateIsSatisfied(CheckRequirement checkReq)
        {
            var req = checkReq.StageRequirement;
            checkReq.IsSatisfied = checkReq.CheckedValue >= req.MinValue && checkReq.CheckedValue <= req.MaxValue;
        }
    }
}