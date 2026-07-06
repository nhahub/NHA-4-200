using System;
using System.Collections.Generic;
using System.Text;
using ZoneSync.Core.Entities.CropModule;
using ZoneSync.Core.Entities.StageRequirementModule;

namespace ZoneSync.Core.Entities.GrowthStageModule
{
    public class GrowthStage
    {
        public int StageId { get; set; }
        public int CropId { get; set; }
        public string StageName { get; set; }
        public int StageOrder { get; set; }
        public int StageDuration { get; set; } // Duration in days

        public Crop Crop { get; set; }
        public ICollection<StageRequirement> StageRequirements { get; set; } = new List<StageRequirement>();

    }
}
