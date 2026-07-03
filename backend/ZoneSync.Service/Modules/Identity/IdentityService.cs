using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Data;
using ZoneSync.Core.Entities.Identity;
using ZoneSync.Core.Enums;
using ZoneSync.Service.Contracts;

namespace ZoneSync.Service.Modules.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public IdentityService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<UserProfile> RegisterOwnerAsync(string email, string password, string firstName, string lastName, string? phoneNumber, string? ssn)
        {
            var applicationUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(applicationUser, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException(errors);
            }

            var profile = new UserProfile
            {
                AspNetUserId = applicationUser.Id,
                Email = email,
                UserFirstName = firstName,
                UserLastName = lastName,
                SSN = ssn,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            context.UserProfiles.Add(profile);
            await context.SaveChangesAsync();

            return profile;
        }

        public async Task<bool> PasswordSignInAsync(string email, string password, bool rememberMe)
        {
            var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, false);

            if (!result.Succeeded)
            {
                return false;
            }

            var profile = await context.UserProfiles
                .FirstOrDefaultAsync(user => user.Email == email);

            if (profile is not null)
            {
                profile.LastLogin = DateTime.Now;
                await context.SaveChangesAsync();
            }

            return true;
        }

        public async Task SignOutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<Invitation> CreateInvitationAsync(int sentByUserId, int? farmId, string invitedEmail, string? invitedPhone, FarmRoleType invitedRole)
        {
            var invitation = new Invitation
            {
                InvitationName = $"{invitedRole} Invitation",
                InvitedEmail = invitedEmail,
                InvitedPhone = invitedPhone,
                InvitedRole = invitedRole,
                InvitationToken = Guid.NewGuid().ToString("N"),
                VerificationCode = Random.Shared.Next(100000, 999999).ToString(),
                InvitationStatus = InvitationStatus.Pending,
                CreatedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddDays(3),
                SentByUserId = sentByUserId,
                FarmId = farmId
            };

            context.Invitations.Add(invitation);
            await context.SaveChangesAsync();

            return invitation;
        }

        public async Task<Invitation?> AcceptInvitationAsync(string token, string verificationCode, int receivedByUserId)
        {
            var invitation = await context.Invitations
                .FirstOrDefaultAsync(item =>
                    item.InvitationToken == token &&
                    item.VerificationCode == verificationCode &&
                    item.InvitationStatus == InvitationStatus.Pending);

            if (invitation is null || invitation.ExpiredAt < DateTime.Now)
            {
                return null;
            }

            invitation.InvitationStatus = InvitationStatus.Accepted;
            invitation.AcceptedAt = DateTime.Now;
            invitation.ReceivedByUserId = receivedByUserId;

            if (invitation.FarmId.HasValue)
            {
                await AddFarmMembershipAsync(invitation.FarmId.Value, receivedByUserId, invitation.InvitedRole);
            }

            await context.SaveChangesAsync();

            return invitation;
        }

        public async Task<FarmMembership> AddFarmMembershipAsync(int farmId, int userId, FarmRoleType roleType)
        {
            var membership = await context.FarmMemberships.FindAsync(farmId, userId);

            if (membership is not null)
            {
                membership.RoleType = roleType;
                return membership;
            }

            membership = new FarmMembership
            {
                FarmId = farmId,
                UserId = userId,
                RoleType = roleType,
                JoinedAt = DateTime.Now
            };

            context.FarmMemberships.Add(membership);

            return membership;
        }
    }
}
