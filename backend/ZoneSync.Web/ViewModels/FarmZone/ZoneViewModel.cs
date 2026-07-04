using System.ComponentModel.DataAnnotations;
using ZoneSync.Core.Enums;

namespace ZoneSync.Web.ViewModels.FarmZone
{
    public class CreateZoneViewModel
    {
        [Required]
        [Display(Name = "Farm")]
        public int FarmId { get; set; }

        // TODO (team-wide gap, same as CreateFarmViewModel.OwnerUserId): should come
        // from the logged-in session, not a manual field.
        [Required]
        [Display(Name = "Created By User Id")]
        public int CreatedByUserId { get; set; }

        [Required]
        [Display(Name = "Zone Name")]
        public string ZoneName { get; set; } = null!;

        [Required]
        [Display(Name = "Area")]
        public decimal ZoneArea { get; set; }

        // Per the task doc: Zone Create screen must let the owner pick assigned
        // users and a supervisor at creation time. Populate the dropdown/checkbox
        // list for these two in the controller from FarmMembership for this Farm.
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

    // Used on the Zone Details view to display assigned users and supervisor —
    // read-only, not a form submission.
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