using System.ComponentModel.DataAnnotations;

namespace ZoneSync.Web.ViewModels.Account
{
    public class AcceptInvitationViewModel
    {
        [Required]
        public string Token { get; set; } = null!;

        [Required]
        [Display(Name = "Verification Code")]
        public string VerificationCode { get; set; } = null!;

        [Required]
        [Display(Name = "Received By User Id")]
        public int ReceivedByUserId { get; set; }
    }
}
