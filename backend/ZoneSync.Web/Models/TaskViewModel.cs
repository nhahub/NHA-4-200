using System.ComponentModel.DataAnnotations;
using ZoneSync.Core.Enums;

namespace ZoneSync.Web.Models
{
    public class TaskFromAlertViewModel
    {
        [Required]
        public int AlertId { get; set; }
        public int ZoneId { get; set; }

        [Required, MaxLength(50)]
        public string TaskName { get; set; } = string.Empty;
        public string? TaskDesc { get; set; }

        [Required]
        public DateOnly DueDate { get; set; }

        public int? ActualVerificationAfterHours { get; set; }

        [Required]
        public int CreatedByUserId { get; set; } // نفس الملحوظة فوق
    }

    public class TaskManualViewModel
    {
        [Required]
        public int ZoneId { get; set; }
        public int? CropPlanId { get; set; }

        [Required, MaxLength(50)]
        public string TaskName { get; set; } = string.Empty;
        public string? TaskDesc { get; set; }

        [Required]
        public DateOnly DueDate { get; set; }

        [Required]
        public TaskPriority TaskPriority { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }
    }
}