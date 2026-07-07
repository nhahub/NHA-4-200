using System;
using System.Collections.Generic;
using System.Text;
using ZoneSync.Core.Entities.GrowthStageModule;

namespace ZoneSync.Core.Entities.StageRequirementModule
{
    public class StageRequirement
    {
        public int ReqId { get; set; }
        public int StageId { get; set; }
        public string ReqName { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string ApplicablePeriod { get; set; }
        public int DefaultVerificationAfterHours { get; set; }
        public bool IsChosenByUser { get; set; }

        public GrowthStage GrowthStage { get; set; }

    }
}
