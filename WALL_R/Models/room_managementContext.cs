using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WALL_R.Models
{
    public partial class room_managementContext : DbContext
    {
        public room_managementContext()
        {
        }

        public room_managementContext(DbContextOptions<room_managementContext> options)
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
        public virtual DbSet<States> States { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=LAPTOP-DKOVRUU6\\SQLEXPRESS;Database=room_management;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accounts>(entity =>
            {
                entity.ToTable("accounts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Ldapid)
                    .HasColumnName("LDAPID")
                    .HasMaxLength(255);

                entity.Property(e => e.Prename)
                    .HasColumnName("prename")
                    .HasMaxLength(255);

                entity.Property(e => e.RightGroupId).HasColumnName("right_group_id");

                entity.Property(e => e.Surname)
                    .HasColumnName("surname")
                    .HasMaxLength(255);

                /*entity.HasOne(d => d.RightGroup)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RightGroupId)
                    .HasConstraintName("FK_right_group_id");*/
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

                /*entity.HasOne(d => d.ComponentType)
                    .WithMany(p => p.Components)
                    .HasForeignKey(d => d.ComponentTypeId)
                    .HasConstraintName("component_type_id");*/
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

                entity.Property(e => e.PriorityId).HasColumnName("priority_id");

                entity.Property(e => e.StateId).HasColumnName("state_id");

                entity.Property(e => e.WriterComment)
                    .HasColumnName("writer_comment")
                    .HasMaxLength(255);

                entity.Property(e => e.WriterId).HasColumnName("writer_id");

                /*entity.HasOne(d => d.Component)
                    .WithMany(p => p.Defects)
                    .HasForeignKey(d => d.ComponentId)
                    .HasConstraintName("FK_component_id");

                entity.HasOne(d => d.DefectType)
                    .WithMany(p => p.Defects)
                    .HasForeignKey(d => d.DefectTypeId)
                    .HasConstraintName("FK_defect_type_id");

                entity.HasOne(d => d.Priority)
                    .WithMany(p => p.Defects)
                    .HasForeignKey(d => d.PriorityId)
                    .HasConstraintName("FK_priority_id");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.Defects)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("FK_state_id");

                entity.HasOne(d => d.Writer)
                    .WithMany(p => p.Defects)
                    .HasForeignKey(d => d.WriterId)
                    .HasConstraintName("FK_writer_id");*/
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

                /*entity.HasOne(d => d.DeviceType)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.DeviceTypeId)
                    .HasConstraintName("FK_device_type_id");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Devices)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK_room_id");*/
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

                entity.HasOne(d => d.RightGroup)
                    .WithMany(p => p.RightGroupsRights)
                    .HasForeignKey(d => d.RightGroupId)
                    .HasConstraintName("FK_right_group_id2");

                entity.HasOne(d => d.Right)
                    .WithMany(p => p.RightGroupsRights)
                    .HasForeignKey(d => d.RightId)
                    .HasConstraintName("FK_right_id");
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

                /*entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK_owner_id");

                entity.HasOne(d => d.PictureFile)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.PictureFileId)
                    .HasConstraintName("FK_picture_file_id");*/
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
