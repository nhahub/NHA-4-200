using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZoneSync.Core.Entities.GrowthStageModule;

namespace ZoneSync.Core.Entities.StageRequirementModule
{
    public class StageRequirement
    {
        [Key]
        public int ReqId { get; set; }
        public int StageId { get; set; }
        public string ReqName { get; set; } = string.Empty;
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string ApplicablePeriod { get; set; } = string.Empty;
        public int DefaultVerificationAfterHours { get; set; }
        public bool IsChosenByUser { get; set; }

        public GrowthStage? GrowthStage { get; set; }

    }
}
