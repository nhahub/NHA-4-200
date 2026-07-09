using System;
using ZoneSync.Core.Entities.Identity;

namespace ZoneSync.Core.Entities.AlertsTasks
{
    public class TaskUser
    {
        public int TaskId { get; set; }
        public TaskItem Task { get; set; } = null!;

        public int UserId { get; set; }
        public UserProfile User { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.Now;
    }
}