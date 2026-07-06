using System;
using System.Collections.Generic;
using System.Text;
using ZoneSync.Core.Entities.CropPlanModule;
using ZoneSync.Core.Entities.GrowthStageModule;

namespace ZoneSync.Core.Entities.CropModule
{
   public class Crop
    {
        public int CropId { get; set; }
        public string CropName { get; set; }
        public string CropSeason { get; set; }
        public string CropCategory { get; set; }
        public string IrrigationType { get; set; }

        public ICollection<GrowthStage> GrowthStages { get; set; } = new List<GrowthStage>();
        public ICollection<CropPlan> CropPlans { get; set; } = new List<CropPlan>();

    }
}
