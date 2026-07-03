using ZoneSync.Core.Enums;

namespace ZoneSync.Core.Entities.Identity
{
    public class Invitation
    {
        public int InvitationId { get; set; }
        public string InvitationName { get; set; } = null!;
        public string InvitedEmail { get; set; } = null!;
        public string? InvitedPhone { get; set; }
        public FarmRoleType InvitedRole { get; set; }
        public string InvitationToken { get; set; } = null!;
        public string VerificationCode { get; set; } = null!;
        public InvitationStatus InvitationStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        public DateTime? AcceptedAt { get; set; }

        public int SentByUserId { get; set; }
        public int? ReceivedByUserId { get; set; }
        public int? FarmId { get; set; }

        public UserProfile SentByUser { get; set; } = null!;
        public UserProfile? ReceivedByUser { get; set; }
    }
}
