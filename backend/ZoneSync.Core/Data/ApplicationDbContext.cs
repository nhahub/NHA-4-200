using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Entities;
using ZoneSync.Core.Entities.CropModule;
using ZoneSync.Core.Entities.CropPlanModule;
using ZoneSync.Core.Entities.GrowthStageModule;
using ZoneSync.Core.Entities.Identity;
using ZoneSync.Core.Entities.StageInformationModule;
using ZoneSync.Core.Entities.StageRequirementModule;

namespace ZoneSync.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Invitation> Invitations => Set<Invitation>();
        public DbSet<FarmMembership> FarmMemberships => Set<FarmMembership>();

        
        public DbSet<Zone> Zones => Set<Zone>();

        public DbSet<Crop> Crops => Set<Crop>();
        public DbSet<GrowthStage> GrowthStages => Set<GrowthStage>();
        public DbSet<StageRequirement> StageRequirements => Set<StageRequirement>();
        public DbSet<CropPlan> CropPlans => Set<CropPlan>();
        public DbSet<StageInformation> StageInformations => Set<StageInformation>();
        public DbSet<CheckRequirement> CheckRequirements => Set<CheckRequirement>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region User Profile

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(user => user.PhoneNumber)
                .IsUnique()
                .HasFilter("[PhoneNumber] IS NOT NULL");

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("User");

                entity.HasKey(user => user.UserId);

                entity.Property(user => user.Email)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(user => user.UserFirstName)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(user => user.UserLastName)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(user => user.SSN)
                    .HasMaxLength(14);

                entity.Property(user => user.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(user => user.AspNetUserId)
                    .IsUnique();

                entity.HasIndex(user => user.SSN)
                    .IsUnique()
                    .HasFilter("[SSN] IS NOT NULL");

                entity.HasOne(user => user.ApplicationUser)
                    .WithOne()
                    .HasForeignKey<UserProfile>(user => user.AspNetUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region Invitation

            modelBuilder.Entity<Invitation>(entity =>
            {
                entity.ToTable("Invitation");

                entity.HasKey(invitation => invitation.InvitationId);

                entity.Property(invitation => invitation.InvitationName)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(invitation => invitation.InvitedEmail)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(invitation => invitation.InvitedPhone)
                    .HasMaxLength(20);

                entity.Property(invitation => invitation.InvitationToken)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(invitation => invitation.VerificationCode)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(invitation => invitation.InvitedRole)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(invitation => invitation.InvitationStatus)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.HasIndex(invitation => invitation.InvitationToken)
                    .IsUnique();

                entity.HasOne(invitation => invitation.SentByUser)
                    .WithMany(user => user.SentInvitations)
                    .HasForeignKey(invitation => invitation.SentByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(invitation => invitation.ReceivedByUser)
                    .WithMany(user => user.ReceivedInvitations)
                    .HasForeignKey(invitation => invitation.ReceivedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region Farm Membership

            modelBuilder.Entity<FarmMembership>(entity =>
            {
                entity.ToTable("FarmMembership");

                entity.HasKey(membership => new { membership.FarmId, membership.UserId });

                entity.Property(membership => membership.RoleType)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(membership => membership.JoinedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(membership => membership.UserProfile)
                    .WithMany(user => user.FarmMemberships)
                    .HasForeignKey(membership => membership.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region Crop Module

            modelBuilder.Entity<GrowthStage>()
                .HasOne(g => g.Crop)
                .WithMany(c => c.GrowthStages)
                .HasForeignKey(g => g.CropId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StageRequirement>()
                .HasOne(s => s.GrowthStage)
                .WithMany(g => g.StageRequirements)
                .HasForeignKey(s => s.StageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CropPlan>()
                .HasOne(cp => cp.Crop)
                .WithMany(c => c.CropPlans)
                .HasForeignKey(cp => cp.CropId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CropPlan>()
                .HasOne(cp => cp.Zone)
                .WithMany()
                .HasForeignKey(cp => cp.ZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StageInformation>()
                .HasOne(si => si.GrowthStage)
                .WithMany()
                .HasForeignKey(si => si.StageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StageInformation>()
                .HasOne(si => si.CropPlan)
                .WithMany(cp => cp.StageInformations)
                .HasForeignKey(si => si.CropPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckRequirement>()
                .HasOne(cr => cr.StageRequirement)
                .WithMany()
                .HasForeignKey(cr => cr.RequirementId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckRequirement>()
                .HasOne(cr => cr.CropPlan)
                .WithMany(cp => cp.CheckRequirements)
                .HasForeignKey(cr => cr.CropPlanId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
