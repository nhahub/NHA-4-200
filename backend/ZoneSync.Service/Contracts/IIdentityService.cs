using ZoneSync.Core.Entities.Identity;
using ZoneSync.Core.Enums;

namespace ZoneSync.Service.Contracts
{
    public interface IIdentityService
    {
        Task<UserProfile> RegisterOwnerAsync(string email, string password, string firstName, string lastName, string? phoneNumber, string? ssn);
        Task<bool> PasswordSignInAsync(string email, string password, bool rememberMe);
        Task SignOutAsync();
        Task<Invitation> CreateInvitationAsync(int sentByUserId, int? farmId, string invitedEmail, string? invitedPhone, FarmRoleType invitedRole);
        Task<Invitation?> AcceptInvitationAsync(string token, string verificationCode, int receivedByUserId);
        Task<FarmMembership> AddFarmMembershipAsync(int farmId, int userId, FarmRoleType roleType);
    }
}
