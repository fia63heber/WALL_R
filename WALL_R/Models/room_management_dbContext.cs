using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WALL_R.Models
{
    public partial class room_management_dbContext : DbContext
    {
        public room_management_dbContext()
        {
        }

        public room_management_dbContext(DbContextOptions<room_management_dbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Accounts> Accounts { get; set; }
        public virtual DbSet<Components> Components { get; set; }
        public virtual DbSet<ComponentTypes> ComponentTypes { get; set; }
        public virtual DbSet<Defects> Defects { get; set; }
        public virtual DbSet<DefectTypes> DefectTypes { get; set; }
        public virtual DbSet<Devices> Devices { get; set; }
        public virtual DbSet<DeviceTypes> DeviceTypes { get; set; }
        public virtual DbSet<Files> Files { get; set; }
        public virtual DbSet<Priorities> Priorities { get; set; }
        public virtual DbSet<RightGroups> RightGroups { get; set; }
        public virtual DbSet<RightGroupsRights> RightGroupsRights { get; set; }
        public virtual DbSet<Rights> Rights { get; set; }
        public virtual DbSet<Rooms> Rooms { get; set; }
        public virtual DbSet<Sessions> Sessions { get; set; }
        public virtual DbSet<States> States { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=LAPTOP-DKOVRUU6\\SQLEXPRESS;Database=room_management_db;Trusted_Connection=True;;MultipleActiveResultSets=true");
                //optionsBuilder.UseSqlServer("Server=(localdb)\\room;Database=room_management;Trusted_Connection=True;;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accounts>(entity =>
            {
                entity.ToTable("accounts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255);

                entity.Property(e => e.Ldapid)
                    .HasColumnName("LDAPID")
                    .HasMaxLength(255);

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(255);

                entity.Property(e => e.Prename)
                    .HasColumnName("prename")
                    .HasMaxLength(255);

                entity.Property(e => e.RightGroupId).HasColumnName("right_group_id");

                entity.Property(e => e.Surname)
                    .HasColumnName("surname")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Components>(entity =>
            {
                entity.ToTable("components");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ComponentTypeId).HasColumnName("component_type_id");

                entity.Property(e => e.DeviceId).HasColumnName("device_id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<ComponentTypes>(entity =>
            {
                entity.ToTable("component_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Defects>(entity =>
            {
                entity.ToTable("defects");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ComponentId).HasColumnName("component_id");

                entity.Property(e => e.DefectTypeId).HasColumnName("defect_type_id");

                entity.Property(e => e.EntryComment)
                    .HasColumnName("entry_comment")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.OwnerComment)
                    .HasColumnName("owner_comment")
                    .HasMaxLength(255);

                entity.Property(e => e.PriorityId).HasColumnName("priority_id");

                entity.Property(e => e.StateId).HasColumnName("state_id");

                entity.Property(e => e.WriterId).HasColumnName("writer_id");
            });

            modelBuilder.Entity<DefectTypes>(entity =>
            {
                entity.ToTable("defect_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Devices>(entity =>
            {
                entity.ToTable("devices");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DeviceTypeId).HasColumnName("device_type_id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.RoomId).HasColumnName("room_id");

                entity.Property(e => e.SerialNumber)
                    .HasColumnName("serial_number")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<DeviceTypes>(entity =>
            {
                entity.ToTable("device_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Files>(entity =>
            {
                entity.ToTable("files");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FilePath)
                    .HasColumnName("file_path")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Priorities>(entity =>
            {
                entity.ToTable("priorities");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<RightGroups>(entity =>
            {
                entity.ToTable("right_groups");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<RightGroupsRights>(entity =>
            {
                entity.ToTable("right_groups_rights");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RightGroupId).HasColumnName("right_group_id");

                entity.Property(e => e.RightId).HasColumnName("right_id");
            });

            modelBuilder.Entity<Rights>(entity =>
            {
                entity.ToTable("rights");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Rooms>(entity =>
            {
                entity.ToTable("rooms");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.OwnerId).HasColumnName("owner_id");

                entity.Property(e => e.PictureFileId).HasColumnName("picture_file_id");

                entity.Property(e => e.RoomNumber)
                    .HasColumnName("room_number")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Sessions>(entity =>
            {
                entity.ToTable("sessions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("account_id");

                entity.Property(e => e.ExpiringDate)
                    .HasColumnName("expiring_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<States>(entity =>
            {
                entity.ToTable("states");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });
        }
    }
}
