using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace cvprojekt.Models;

public partial class CvDbContext : DbContext
{
    public CvDbContext()
    {
    }

    public CvDbContext(DbContextOptions<CvDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cv> Cvs { get; set; }

    public virtual DbSet<CvView> CvViews { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cv>(entity =>
        {
            entity.HasKey(e => e.Cvid).HasName("PK__CV__A04CFC4375B650F9");

            entity.ToTable("CV");

            entity.Property(e => e.Cvid).HasColumnName("CVID");

            entity.HasOne(d => d.User).WithMany(p => p.Cvs)
                .HasForeignKey(d => d.Owner)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CV__Owner__440B1D61");

            entity.HasMany(d => d.Projects).WithMany(p => p.Cvs)
                .UsingEntity<Dictionary<string, object>>(
                    "CvProject",
                    r => r.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CvProject__Proje__47DBAE45"),
                    l => l.HasOne<Cv>().WithMany()
                        .HasForeignKey("Cvid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CvProjects__CVID__46E78A0C"),
                    j =>
                    {
                        j.HasKey("Cvid", "ProjectId").HasName("PK__CvProjec__B72D57AE3F3B2B7C");
                        j.ToTable("CvProjects");
                        j.IndexerProperty<int>("Cvid").HasColumnName("CVID");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("ProjectID");
                    });
        });

        modelBuilder.Entity<CvView>(entity =>
        {
            entity.HasKey(e => e.Cvid).HasName("PK__CvViews__A04CFC431AF4DD6E");

            entity.Property(e => e.Cvid)
                .ValueGeneratedNever()
                .HasColumnName("CVID");

            entity.HasOne(d => d.Cv).WithOne(p => p.CvView)
                .HasForeignKey<CvView>(d => d.Cvid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CvViews__CVID__4BAC3F29");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABED0BA245136");

            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Projects)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Projects__Create__3D5E1FD2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC1796EBA0");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105341D6670E1").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.ProfilePicture).HasMaxLength(100);

            entity.HasMany(d => d.ProjectsNavigation).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserProject",
                    r => r.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserProje__Proje__412EB0B6"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserProje__UserI__403A8C7D"),
                    j =>
                    {
                        j.HasKey("UserId", "ProjectId").HasName("PK__UserProj__00E967414D379F8E");
                        j.ToTable("UserProjects");
                        j.IndexerProperty<int>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("ProjectID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
