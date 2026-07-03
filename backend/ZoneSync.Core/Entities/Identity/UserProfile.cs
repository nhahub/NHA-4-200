namespace ZoneSync.Core.Entities.Identity
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string AspNetUserId { get; set; } = null!;
        public string? SSN { get; set; }
        public string Email { get; set; } = null!;
        public string UserFirstName { get; set; } = null!;
        public string UserLastName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? SoftDeleteAt { get; set; }
        public DateTime? LastLogin { get; set; }

        public ApplicationUser ApplicationUser { get; set; } = null!;
        public ICollection<Invitation> SentInvitations { get; set; } = new HashSet<Invitation>();
        public ICollection<Invitation> ReceivedInvitations { get; set; } = new HashSet<Invitation>();
        public ICollection<FarmMembership> FarmMemberships { get; set; } = new HashSet<FarmMembership>();
    }
}
