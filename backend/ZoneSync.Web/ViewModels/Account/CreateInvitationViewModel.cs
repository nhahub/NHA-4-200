using System.ComponentModel.DataAnnotations;
using ZoneSync.Core.Enums;

namespace ZoneSync.Web.ViewModels.Account
{
    public class CreateInvitationViewModel
    {
        [Required]
        [Display(Name = "Sender User Id")]
        public int SentByUserId { get; set; }

        [Display(Name = "Farm Id")]
        public int? FarmId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Invited Email")]
        public string InvitedEmail { get; set; } = null!;

        [Phone]
        [Display(Name = "Invited Phone")]
        public string? InvitedPhone { get; set; }

        [Required]
        [Display(Name = "Role")]
        public FarmRoleType InvitedRole { get; set; } = FarmRoleType.Engineer;

        public string? CreatedToken { get; set; }
        public string? VerificationCode { get; set; }
    }
}
