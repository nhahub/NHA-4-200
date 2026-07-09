using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZoneSync.Core.Entities.Identity;
using ZoneSync.Core.Entities.FarmZone;
using ZoneSync.Core.Entities.CropPlanModule;

namespace ZoneSync.Core.Entities.AlertsTasks
{
    // الكلاس اسمه TaskItem عشان متتلخبطيش مع System.Threading.Tasks.Task
    // الجدول في الداتابيز اسمه "Task" — هنعمل ToTable("Task") لما نوصل للـ DbContext
    public class TaskItem
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        public int ZoneId { get; set; }
        public Zone Zone { get; set; } = null!;

        public int? CropPlanId { get; set; }
        public CropPlan? CropPlan { get; set; }

        public int? AlertId { get; set; }
        public Alert? Alert { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }
        [ForeignKey(nameof(CreatedByUserId))]
        public UserProfile CreatedByUser { get; set; } = null!;

        [Required, MaxLength(50)]
        public string TaskName { get; set; } = string.Empty;

        public string? TaskDesc { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // خليكي منتبهة: كتبناه بالاسم الكامل عشان التعارض مع System.Threading.Tasks.TaskStatus
        [Required]
        public ZoneSync.Core.Enums.TaskStatus TaskStatus { get; set; } = ZoneSync.Core.Enums.TaskStatus.Pending;

        [Required]
        public DateOnly DueDate { get; set; }

        public DateTime? CompletionTime { get; set; }

        [Required]
        public ZoneSync.Core.Enums.TaskPriority TaskPriority { get; set; }

        public int? ActualVerificationAfterHours { get; set; }

        [Required]
        public ZoneSync.Core.Enums.TaskType TaskType { get; set; }

        public ICollection<TaskUser> AssignedUsers { get; set; } = new List<TaskUser>();

        // 1:1 حقيقي (فيه unique constraint في الـ SQL)
        public ActionLog? ActionLog { get; set; }
    }
}