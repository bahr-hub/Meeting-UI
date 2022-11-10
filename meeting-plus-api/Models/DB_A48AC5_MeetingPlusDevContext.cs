using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MeetingPlus.API.Models
{
    public partial class DB_A48AC5_MeetingPlusDevContext : DbContext
    {
        public DB_A48AC5_MeetingPlusDevContext()
        {
        }

        public DB_A48AC5_MeetingPlusDevContext(DbContextOptions<DB_A48AC5_MeetingPlusDevContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AggregatedCounter> AggregatedCounter { get; set; }
        public virtual DbSet<Hash> Hash { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<JobParameter> JobParameter { get; set; }
        public virtual DbSet<JobQueue> JobQueue { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<List> List { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<Meeting> Meeting { get; set; }
        public virtual DbSet<MeetingParticipant> MeetingParticipant { get; set; }
        public virtual DbSet<MeetingTag> MeetingTag { get; set; }
        public virtual DbSet<MeetingTask> MeetingTask { get; set; }
        public virtual DbSet<MeetingTopic> MeetingTopic { get; set; }
        public virtual DbSet<Module> Module { get; set; }
        public virtual DbSet<ModulePrivilege> ModulePrivilege { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<Privilege> Privilege { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<Proposals> Proposals { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleModulePrivilege> RoleModulePrivilege { get; set; }
        public virtual DbSet<Schema> Schema { get; set; }
        public virtual DbSet<Server> Server { get; set; }
        public virtual DbSet<Set> Set { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<SystemConfiguration> SystemConfiguration { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserConfiguration> UserConfiguration { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Vacation> Vacation { get; set; }

        // Unable to generate entity type for table 'HangFire.Counter'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=SystemConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<AggregatedCounter>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key)
                    .HasMaxLength(100)
                    .ValueGeneratedNever();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Hash>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.StateName)
                    .HasName("IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.HasIndex(e => new { e.StateName, e.ExpireAt })
                    .HasName("IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Arguments).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.InvocationData).IsRequired();

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameter>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameter)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueue>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Text).HasMaxLength(50);
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NameAr)
                    .IsRequired()
                    .HasColumnName("NameAR")
                    .HasMaxLength(50);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.LocationCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Location_Creator");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.LocationUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_Location_Updator");
            });

            modelBuilder.Entity<Meeting>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.MeetingCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Meeting_Creator");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Meeting)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Meeting_Location");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Meeting)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_Meeting_Project");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.MeetingUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_Meeting_Updator");
            });

            modelBuilder.Entity<MeetingParticipant>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.MeetingId).HasColumnName("MeetingID");

                entity.Property(e => e.ParticipantId).HasColumnName("ParticipantID");

                entity.HasOne(d => d.Meeting)
                    .WithMany(p => p.MeetingParticipant)
                    .HasForeignKey(d => d.MeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MeetingParticipant_Meeting");

                entity.HasOne(d => d.Participant)
                    .WithMany(p => p.MeetingParticipant)
                    .HasForeignKey(d => d.ParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MeetingParticipant_User");
            });

            modelBuilder.Entity<MeetingTag>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.MeetingId).HasColumnName("MeetingID");

                entity.Property(e => e.TagId).HasColumnName("TagID");

                entity.HasOne(d => d.Meeting)
                    .WithMany(p => p.MeetingTag)
                    .HasForeignKey(d => d.MeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MeetingTag_Meeting");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.MeetingTag)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MeetingTag_Tag");
            });

            modelBuilder.Entity<MeetingTask>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.AssigneeId).HasColumnName("AssigneeID");

                entity.Property(e => e.MeetingId).HasColumnName("MeetingID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Assignee)
                    .WithMany(p => p.MeetingTaskAssignee)
                    .HasForeignKey(d => d.AssigneeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MeetingTasks_Assignee");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.MeetingTaskCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MeetingTasks_Creator");

                entity.HasOne(d => d.Meeting)
                    .WithMany(p => p.MeetingTask)
                    .HasForeignKey(d => d.MeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MeetingTasks_Meeting");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.MeetingTaskUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_MeetingTasks_Updator");
            });

            modelBuilder.Entity<MeetingTopic>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.FkMeetingId).HasColumnName("FK_MeetingID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PresenterId).HasColumnName("PresenterID");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.MeetingTopicCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Topic_Creator");

                entity.HasOne(d => d.FkMeeting)
                    .WithMany(p => p.MeetingTopic)
                    .HasForeignKey(d => d.FkMeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MeetingTopic_Meeting");

                entity.HasOne(d => d.Presenter)
                    .WithMany(p => p.MeetingTopicPresenter)
                    .HasForeignKey(d => d.PresenterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Topic_Presenter");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.MeetingTopicUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_Topic_Updator");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ModulePrivilege>(entity =>
            {
                entity.HasKey(e => new { e.FkPrivilegeId, e.FkModuleId });

                entity.Property(e => e.FkPrivilegeId).HasColumnName("FK_PrivilegeID");

                entity.Property(e => e.FkModuleId).HasColumnName("FK_ModuleID");

                entity.HasOne(d => d.FkModule)
                    .WithMany(p => p.ModulePrivilege)
                    .HasForeignKey(d => d.FkModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Privilege_Module");

                entity.HasOne(d => d.FkPrivilege)
                    .WithMany(p => p.ModulePrivilege)
                    .HasForeignKey(d => d.FkPrivilegeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Privilege_Privilege");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notification)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_User");
            });

            modelBuilder.Entity<Privilege>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ProjectCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Project_Creator");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.ProjectUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_Project_Updator");
            });

            modelBuilder.Entity<Proposals>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Meeting)
                    .WithMany(p => p.Proposals)
                    .HasForeignKey(d => d.MeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Proposals_Meeting");

                entity.HasOne(d => d.ProposedByNavigation)
                    .WithMany(p => p.Proposals)
                    .HasForeignKey(d => d.ProposedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Proposals_User");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.RoleCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Role_Creator");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.RoleUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_Role_Updator");
            });

            modelBuilder.Entity<RoleModulePrivilege>(entity =>
            {
                entity.HasKey(e => new { e.FkPrivilegeId, e.FkModuleId, e.FkRoleId })
                    .HasName("PK_UserGroupPrivilege");

                entity.Property(e => e.FkPrivilegeId).HasColumnName("FK_PrivilegeID");

                entity.Property(e => e.FkModuleId).HasColumnName("FK_ModuleID");

                entity.Property(e => e.FkRoleId).HasColumnName("FK_RoleID");

                entity.HasOne(d => d.FkModule)
                    .WithMany(p => p.RoleModulePrivilege)
                    .HasForeignKey(d => d.FkModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePrivilege_Module");

                entity.HasOne(d => d.FkPrivilege)
                    .WithMany(p => p.RoleModulePrivilege)
                    .HasForeignKey(d => d.FkPrivilegeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePrivilege_Privilege");

                entity.HasOne(d => d.FkRole)
                    .WithMany(p => p.RoleModulePrivilege)
                    .HasForeignKey(d => d.FkRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePrivilege_Role");
            });

            modelBuilder.Entity<Schema>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<Server>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat)
                    .HasName("IX_HangFire_Server_LastHeartbeat");

                entity.Property(e => e.Id)
                    .HasMaxLength(100)
                    .ValueGeneratedNever();

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt)
                    .HasName("IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score })
                    .HasName("IX_HangFire_Set_Score");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.State)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            modelBuilder.Entity<SystemConfiguration>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.TimeZone)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.TagCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Tag_Creator");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.TagUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_Tag_Updator");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.FkUserConfigurationId)
                    .HasName("IX_User_FK_UserConfigurationID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.FkUserProfileId).HasColumnName("FK_UserProfileID");

                entity.Property(e => e.Mobile).HasMaxLength(50);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.InverseCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_User_CreatedBy");

                entity.HasOne(d => d.FkUserConfiguration)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.FkUserConfigurationId)
                    .HasConstraintName("FK_User_UserConfiguration");

                entity.HasOne(d => d.FkUserProfile)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.FkUserProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_UserProfile");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.InverseUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_User_ModifiedBy");
            });

            modelBuilder.Entity<UserConfiguration>(entity =>
            {
                entity.HasIndex(e => e.LanguageId)
                    .HasName("IX_UserConfiguration_FK_LanguageID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.LanguageId).HasColumnName("LanguageID");

                entity.Property(e => e.TimeZone).HasMaxLength(50);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CountryCode).HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.UserProfileCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_UserProfile_CreatedBy");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.UserProfileUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_UserProfile_ModifiedBy");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.FkUserId, e.FkRoleId })
                    .HasName("PK_UserGroupUser");

                entity.Property(e => e.FkUserId).HasColumnName("FK_UserID");

                entity.Property(e => e.FkRoleId).HasColumnName("FK_RoleID");

                entity.HasOne(d => d.FkRole)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.FkRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRole_Role");

                entity.HasOne(d => d.FkUser)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.FkUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRole_User");
            });

            modelBuilder.Entity<Vacation>(entity =>
            {
                entity.HasIndex(e => e.CreatedBy)
                    .HasName("IX_Vacations_CreatedBy");

                entity.HasIndex(e => e.FkUserId)
                    .HasName("IX_Vacations_FK_UserID");

                entity.HasIndex(e => e.UpdatedBy)
                    .HasName("IX_Vacations_UpdatedBy");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.FkUserId).HasColumnName("FK_UserID");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.VacationCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Vacations_CreatedBy");

                entity.HasOne(d => d.FkUser)
                    .WithMany(p => p.VacationFkUser)
                    .HasForeignKey(d => d.FkUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Vacations_User");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.VacationUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_Vacations_ModifiedBy");
            });
        }
    }
}
