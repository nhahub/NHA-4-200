using ZoneSync.Core.Entities.FarmZone;
using System;
using System.Collections.Generic;
using System.Text;
using ZoneSync.Core.Entities.CropModule;
using ZoneSync.Core.Entities.StageInformationModule;

namespace ZoneSync.Core.Entities.CropPlanModule
{
    public class CropPlan
    {
        public int CropPlanId { get; set; }
        public int CropId { get; set; }
        public int ZoneId { get; set; }
        public DateTime PlantingDate { get; set; }
        public DateTime PredictedHarvestTime { get; set; }
        public DateTime? ActualHarvestTime { get; set; }

        public Crop Crop { get; set; }
        public Zone Zone { get; set; }
        public ICollection<StageInformation> StageInformations { get; set; } = new List<StageInformation>();
        public ICollection<CheckRequirement> CheckRequirements { get; set; } = new List<CheckRequirement>();

    }
}
