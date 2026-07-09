using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZoneSync.Core.Entities.Identity;

namespace ZoneSync.Core.Entities.AlertsTasks
{
    public class ActionLog
    {
        [Key]
        public int ActionLogId { get; set; }

        [Required]
        public int TaskId { get; set; }
        public TaskItem Task { get; set; } = null!;

        [Required]
        public int ExecutedByUserId { get; set; }
        [ForeignKey(nameof(ExecutedByUserId))]
        public UserProfile ExecutedByUser { get; set; } = null!;

        [MaxLength(50)]
        public string? QuantityType { get; set; }

        public string? QuantityDesc { get; set; }

        [Required]
        public DateTime ExecutedAt { get; set; } = DateTime.Now;

        public string? Result { get; set; }
        public string? Notes { get; set; }
    }
}