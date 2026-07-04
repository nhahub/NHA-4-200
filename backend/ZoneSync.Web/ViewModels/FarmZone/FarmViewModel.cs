using System.ComponentModel.DataAnnotations;

namespace ZoneSync.Web.ViewModels.FarmZone
{
    public class CreateFarmViewModel
    {
        // TODO (team-wide gap, not specific to this module): this should come from
        // the logged-in user's session, not a manual form field. Matches the same
        // temporary pattern currently used in CreateInvitationViewModel.SentByUserId —
        // flag to the team before submission.
        [Required]
        [Display(Name = "Owner User Id")]
        public int OwnerUserId { get; set; }

        [Required]
        [Display(Name = "Farm Name")]
        public string FarmName { get; set; } = null!;

        [Required]
        [Display(Name = "Farm Location")]
        public string FarmLocation { get; set; } = null!;

        [Required]
        [Display(Name = "Soil Type")]
        public string SoilType { get; set; } = null!;
    }

    public class EditFarmViewModel
    {
        public int FarmId { get; set; }

        [Required]
        [Display(Name = "Farm Name")]
        public string FarmName { get; set; } = null!;

        [Required]
        [Display(Name = "Farm Location")]
        public string FarmLocation { get; set; } = null!;

        [Required]
        [Display(Name = "Soil Type")]
        public string SoilType { get; set; } = null!;
    }
}