using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZoneSync.Core.Entities.FarmZone;
using ZoneSync.Core.Entities.Identity;

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

            #region Farm
            modelBuilder.Entity<Farm>(entity =>
            {
                entity.ToTable("Farm"); // EF's default would pluralize to "Farms" — must match ZoneSyncDb.sql exactly

                entity.HasKey(f => f.FarmId);

                entity.Property(f => f.TotalArea)
                    .HasPrecision(10, 2); // matches dec(10,2) in ZoneSyncDb.sql — EF's default is decimal(18,2), must override

                entity.Property(f => f.FarmName).HasMaxLength(50);
                entity.Property(f => f.FarmLocation).HasMaxLength(100);
                entity.Property(f => f.SoilType).HasMaxLength(100);

                // Farm -> OwnerUser (FK: OwnerUserId -> User.UserId)
                entity.HasOne(f => f.OwnerUser)
                    .WithMany() // Member 1's User entity doesn't need a "Farms owned" collection unless they want one
                    .HasForeignKey(f => f.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict); // don't let a User delete cascade-wipe Farms

                // Farm has many Zones
                entity.HasMany(f => f.Zones)
                    .WithOne(z => z.Farm)
                    .HasForeignKey(z => z.FarmId)
                    .OnDelete(DeleteBehavior.Restrict); // soft-delete only — never let EF cascade-delete real rows
            });

            #endregion

            #region Zone
            modelBuilder.Entity<Zone>(entity =>
            {
                entity.ToTable("Zone");

                entity.HasKey(z => z.ZoneId);

                entity.Property(z => z.ZoneArea)
                    .HasPrecision(10, 2);

                entity.Property(z => z.ZoneName).HasMaxLength(50);

                // Store the ZoneStatus enum as its string name ('Available', 'Planted',
                // 'Inactive'), not EF's default int (0, 1, 2). Must match ZoneSyncDb.sql
                // exactly so raw queries / other modules reading this column see the
                // same values the rest of the team expects.
                entity.Property(z => z.ZoneStatus)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                // Mirrors CHK_Zone_Status in ZoneSyncDb.sql. EF Core does NOT generate
                // CHECK constraints from C# enums or attributes — must be added explicitly.
                entity.HasCheckConstraint(
                    "CHK_Zone_Status",
                    "[ZoneStatus] IN ('Available', 'Planted', 'Inactive')");

                // Zone -> CreatedByUser (FK: CreatedByUserId -> User.UserId)
                entity.HasOne(z => z.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(z => z.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Zone -> Supervisor (FK: SupervisorId -> User.UserId, nullable)
                // Both this and CreatedByUser point at User — EF Core cannot tell them
                // apart by convention alone. Each needs its own explicit HasOne/
                // HasForeignKey pair with a distinct FK column, or EF will throw a
                // "multiple relationships" configuration error at startup.
                entity.HasOne(z => z.Supervisor)
                    .WithMany()
                    .HasForeignKey(z => z.SupervisorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false); // SupervisorId is nullable — a zone may have no supervisor yet
            });

            #endregion

            #region ZoneUser
            modelBuilder.Entity<ZoneUser>(entity =>
            {
                entity.ToTable("ZoneUser");

                // Composite PK — cannot be expressed via data annotations, must be Fluent API.
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
                entity.ToTable("EntityActivityLog");

                entity.HasKey(e => e.ActivityId);

                entity.Property(e => e.EntityType)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(e => e.ActionType)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.HasCheckConstraint(
                    "CHK_EntityActivityLog_EntityType",
                    "[EntityType] IN ('Zone', 'CropPlan', 'Task')");

                entity.HasCheckConstraint(
                    "CHK_EntityActivityLog_ActionType",
                    "[ActionType] IN ('Create', 'Update', 'Delete')");

                // EntityId is intentionally left unconfigured beyond its column type —
                // it is a polymorphic reference (target table depends on EntityType) and
                // must NOT be given a foreign key or navigation property. EF Core cannot
                // represent this relationship; it stays a plain int, written to manually
                // by FarmZoneService.WriteActivityLog.

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion
        }
    }
}
