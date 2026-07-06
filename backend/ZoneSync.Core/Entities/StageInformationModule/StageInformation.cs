using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ZoneSync.Core.Entities.CropPlanModule;
using ZoneSync.Core.Entities.GrowthStageModule;

namespace ZoneSync.Core.Entities.StageInformationModule
{
    internal class StageInformation
    {
        
            [Key]
            public int Id { get; set; } 
            public int StageId { get; set; }
            public int CropPlanId { get; set; }

            public DateTime PredictedStartDate { get; set; }  

            public DateTime PredictedEndDate { get; set; }

            public DateTime? ActualStartDate { get; set; } 
            public DateTime? ActualEndDate { get; set; } 
            public string DelayDescription { get; set; } 

            public string StageStatus { get; set; } 

            [ForeignKey("StageId")]
            public GrowthStage GrowthStage { get; set; }

            [ForeignKey("CropPlanId")]
            public CropPlan CropPlan { get; set; }
        public string StageName { get; set;
    }
}
