using System.ComponentModel.DataAnnotations;
using ZoneSync.Core.Enums;

namespace ZoneSync.Web.ViewModels.FarmZone
{
    public class CreateZoneViewModel
    {
        [Required]
        [Display(Name = "Farm")]
        public int FarmId { get; set; }

        
        [Required]
        [Display(Name = "Zone Name")]
        public string ZoneName { get; set; } = null!;

        [Required]
        [Display(Name = "Area")]
        public decimal ZoneArea { get; set; }

        [Display(Name = "Assigned Users")]
        public List<int> AssignedUserIds { get; set; } = new();

        [Display(Name = "Supervisor")]
        public int? SupervisorUserId { get; set; }
    }

    public class EditZoneViewModel
    {
        public int ZoneId { get; set; }

        [Required]
        [Display(Name = "Zone Name")]
        public string ZoneName { get; set; } = null!;

        [Required]
        [Display(Name = "Area")]
        public decimal ZoneArea { get; set; }

        [Required]
        [Display(Name = "Status")]
        public ZoneStatus ZoneStatus { get; set; }
    }

   public class ZoneDetailsViewModel
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = null!;
        public decimal ZoneArea { get; set; }
        public ZoneStatus ZoneStatus { get; set; }
        public string FarmName { get; set; } = null!;

        public List<string> AssignedUserNames { get; set; } = new();
        public string? SupervisorName { get; set; }
    }
}