using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;
using ZoneSync.Core.Entities.CropPlanModule;
using ZoneSync.Core.Entities.StageRequirementModule;

namespace ZoneSync.Core.Entities
{
    public class CheckRequirement
    {
        [Key]
        public int CheckId { get; set; }
        public int RequirementId { get; set; }
        public int CropPlanId { get; set; }
        public decimal CheckedValue { get; set; }
        public DateTime LastCheckDate { get; set; }
        public DateTime? NextCheckDate { get; set; }
        public bool IsSatisfied { get; set; }

        public StageRequirement StageRequirement { get; set; }
        public CropPlan CropPlan { get; set; }
    }
}
