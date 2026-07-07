using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZoneSync.Core.Entities.CropPlanModule;
using ZoneSync.Core.Entities.GrowthStageModule;

namespace ZoneSync.Core.Entities.CropModule
{
   public class Crop
    {
        [Key]
        public int CropId { get; set; }
        public string CropName { get; set; } = string.Empty;
        public string CropSeason { get; set; } = string.Empty;
        public string CropCategory { get; set; } = string.Empty;
        public string IrrigationType { get; set; } = string.Empty;

        public ICollection<GrowthStage> GrowthStages { get; set; } = new List<GrowthStage>();
        public ICollection<CropPlan> CropPlans { get; set; } = new List<CropPlan>();

    }
}
