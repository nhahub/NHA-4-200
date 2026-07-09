using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZoneSync.Core.Entities;
using ZoneSync.Core.Entities.Identity;
using ZoneSync.Core.Entities.FarmZone;
using ZoneSync.Core.Entities.CropPlanModule;
using ZoneSync.Core.Entities.StageRequirementModule;
using ZoneSync.Core.Entities.Sensors;
using ZoneSync.Core.Enums;

namespace ZoneSync.Core.Entities.AlertsTasks
{
    public class Alert
    {
        [Key]
        public int AlertId { get; set; }

        [Required]
        public int ZoneId { get; set; }
        public Zone Zone { get; set; } = null!;

        // CropPlan اختياري
        public int? CropPlanId { get; set; }
        public CropPlan? CropPlan { get; set; }

        // بيتملي بس لو الألرت جاي من check فعلي اتنفذ (OutOfRangeReading / FaultySensor)
        public int? CheckId { get; set; }
        public CheckRequirement? CheckRequirement { get; set; }

        // بيتملي دايماً. لوحده (من غير CheckId) في حالة HardwareMissing
        public int? RequirementId { get; set; }
        public StageRequirement? StageRequirement { get; set; }

        public int? SensorInstanceId { get; set; }
        public SensorInstance? SensorInstance { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }
        [ForeignKey(nameof(CreatedByUserId))]
        public UserProfile CreatedByUser { get; set; } = null!;

        public int? ConfirmedByUserId { get; set; }
        [ForeignKey(nameof(ConfirmedByUserId))]
        public UserProfile? ConfirmedByUser { get; set; }

        [Required]
        public AlertType AlertType { get; set; }

        [Required]
        public DateTime FiringDate { get; set; } = DateTime.Now;

        [Required]
        public AlertSeverity AlertSeverity { get; set; }

        [Required]
        public AlertStatus AlertStatus { get; set; } = AlertStatus.UnderReview;

        // ملحوظة: Task.AlertId مش unique في الـ SQL، يعني مفيش 1:1 متفروض
        // على مستوى الداتابيز. بنمثلها هنا كـ collection عشان تبقى مطابقة للسكيما.
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}