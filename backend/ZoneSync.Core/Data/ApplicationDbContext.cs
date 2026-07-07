using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Entities.FarmZone;
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
        public DbSet<Farm> Farms { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<ZoneUser> ZoneUsers { get; set; }
        public DbSet<EntityActivityLog> EntityActivityLogs { get; set; }

        

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
            modelBuilder.Entity<Crop>(entity =>
            {
                entity.ToTable("Crops");
                entity.HasKey(c => c.CropId);
                entity.Property(c => c.CropName).HasMaxLength(100).IsRequired();
                entity.Property(c => c.CropSeason).HasMaxLength(50).IsRequired();
                entity.Property(c => c.CropCategory).HasMaxLength(50).IsRequired();
                entity.Property(c => c.IrrigationType).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<GrowthStage>(entity =>
            {
                entity.ToTable("GrowthStages");
                entity.HasKey(g => g.StageId);
                entity.Property(g => g.StageName).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<StageRequirement>(entity =>
            {
                entity.ToTable("StageRequirements");
                entity.HasKey(s => s.ReqId);
                entity.Property(s => s.ReqName).HasMaxLength(100).IsRequired();
                entity.Property(s => s.MinValue).HasPrecision(10, 2);
                entity.Property(s => s.MaxValue).HasPrecision(10, 2);
                entity.Property(s => s.ApplicablePeriod).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<CropPlan>(entity =>
            {
                entity.ToTable("CropPlans");
                entity.HasKey(cp => cp.CropPlanId);
            });

            modelBuilder.Entity<StageInformation>(entity =>
            {
                entity.ToTable("StageInformations");
                entity.HasKey(si => si.Id);
                entity.Property(si => si.StageName).HasMaxLength(100).IsRequired();
                entity.Property(si => si.StageStatus).HasMaxLength(50).IsRequired();
                entity.Property(si => si.DelayDescription).HasMaxLength(500);
            });

            modelBuilder.Entity<CheckRequirement>(entity =>
            {
                entity.ToTable("CheckRequirements");
                entity.HasKey(cr => cr.CheckId);
                entity.Property(cr => cr.CheckedValue).HasPrecision(10, 2);
            });

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

            modelBuilder.Entity<Crop>().HasData(
                new Crop { CropId = 1, CropName = "Tomato", CropSeason = "Summer", CropCategory = "Vegetable", IrrigationType = "Drip" },
                new Crop { CropId = 2, CropName = "Wheat", CropSeason = "Winter", CropCategory = "Grain", IrrigationType = "Sprinkler" });

            modelBuilder.Entity<GrowthStage>().HasData(
                new GrowthStage { StageId = 1, CropId = 1, StageName = "Germination", StageOrder = 1, StageDuration = 10 },
                new GrowthStage { StageId = 2, CropId = 1, StageName = "Vegetative", StageOrder = 2, StageDuration = 30 },
                new GrowthStage { StageId = 3, CropId = 1, StageName = "Harvest", StageOrder = 3, StageDuration = 20 },
                new GrowthStage { StageId = 4, CropId = 2, StageName = "Tillering", StageOrder = 1, StageDuration = 25 });

            modelBuilder.Entity<StageRequirement>().HasData(
                new StageRequirement { ReqId = 1, StageId = 1, ReqName = "Soil moisture", MinValue = 45, MaxValue = 70, ApplicablePeriod = "Daily", DefaultVerificationAfterHours = 24, IsChosenByUser = true },
                new StageRequirement { ReqId = 2, StageId = 2, ReqName = "Temperature", MinValue = 18, MaxValue = 30, ApplicablePeriod = "Daily", DefaultVerificationAfterHours = 24, IsChosenByUser = true },
                new StageRequirement { ReqId = 3, StageId = 4, ReqName = "Water level", MinValue = 30, MaxValue = 60, ApplicablePeriod = "Weekly", DefaultVerificationAfterHours = 48, IsChosenByUser = true });
            #endregion

            #region Farm
            modelBuilder.Entity<Farm>(entity =>
            {
                entity.ToTable("Farm");

                entity.HasKey(f => f.FarmId);

                entity.Property(f => f.TotalArea)
                    .HasPrecision(10, 2); 

                entity.Property(f => f.FarmName).HasMaxLength(50);
                entity.Property(f => f.FarmLocation).HasMaxLength(100);
                entity.Property(f => f.SoilType).HasMaxLength(100);

                entity.HasOne(f => f.OwnerUser)
                    .WithMany() 
                    .HasForeignKey(f => f.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict); 

                
                entity.HasMany(f => f.Zones)
                    .WithOne(z => z.Farm)
                    .HasForeignKey(z => z.FarmId)
                    .OnDelete(DeleteBehavior.Restrict); // soft-delete only — never let EF cascade-delete real rows
            });

            #endregion

            #region Zone
            modelBuilder.Entity<Zone>(entity =>
            {
                entity.ToTable("Zone", t => t.HasCheckConstraint(
                    "CHK_Zone_Status",
                    "[ZoneStatus] IN ('Available', 'Planted', 'Inactive')"));

                entity.HasKey(z => z.ZoneId);

                entity.Property(z => z.ZoneArea)
                    .HasPrecision(10, 2);

                entity.Property(z => z.ZoneName).HasMaxLength(50);

                entity.Property(z => z.ZoneStatus)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                // Zone -> CreatedByUser (FK: CreatedByUserId -> UserProfile.UserId)
                entity.HasOne(z => z.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(z => z.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Zone -> Supervisor (FK: SupervisorId -> UserProfile.UserId, nullable)
                entity.HasOne(z => z.Supervisor)
                    .WithMany()
                    .HasForeignKey(z => z.SupervisorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });

            #endregion

            #region ZoneUser
            modelBuilder.Entity<ZoneUser>(entity =>
            {
                entity.ToTable("ZoneUser");

                entity.HasKey(zu => new { zu.ZoneId, zu.UserId });

                entity.HasOne(zu => zu.Zone)
                    .WithMany(z => z.ZoneUsers)
                    .HasForeignKey(zu => zu.ZoneId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(zu => zu.User)
                    .WithMany()
                    .HasForeignKey(zu => zu.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region EntityActivityLog
            modelBuilder.Entity<EntityActivityLog>(entity =>
            {
                entity.ToTable("EntityActivityLog", t =>
                {
                    t.HasCheckConstraint(
                        "CHK_EntityActivityLog_EntityType",
                        "[EntityType] IN ('Zone', 'CropPlan', 'Task')");
                    t.HasCheckConstraint(
                        "CHK_EntityActivityLog_ActionType",
                        "[ActionType] IN ('Create', 'Update', 'Delete')");
                });

                entity.HasKey(e => e.ActivityId);

                entity.Property(e => e.EntityType)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(e => e.ActionType)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion
        }
    }
}
