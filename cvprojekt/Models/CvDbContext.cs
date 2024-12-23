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

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Cv> Cvs { get; set; }

    public virtual DbSet<CvView> CvViews { get; set; }

    public virtual DbSet<Education> Educations { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-DF3QFIP;Initial Catalog=CV_DB;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.ProjectsNavigation).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserProject",
                    r => r.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserProje__Proje__3D2915A8"),
                    l => l.HasOne<AspNetUser>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserProje__UserI__3C34F16F"),
                    j =>
                    {
                        j.HasKey("UserId", "ProjectId").HasName("PK__UserProj__00E9674173AFB3B7");
                        j.ToTable("UserProjects");
                        j.IndexerProperty<string>("UserId").HasColumnName("UserID");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("ProjectID");
                    });

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Cv>(entity =>
        {
            entity.HasKey(e => e.Cvid).HasName("PK__CV__A04CFC43EF4B5070");

            entity.ToTable("CV");

            entity.Property(e => e.Cvid).HasColumnName("CVID");
            entity.Property(e => e.Owner).HasMaxLength(450);

            entity.HasOne(d => d.OwnerNavigation).WithMany(p => p.Cvs)
                .HasForeignKey(d => d.Owner)
                .HasConstraintName("FK__CV__Owner__14270015");

            entity.HasMany(d => d.Projects).WithMany(p => p.Cvs)
                .UsingEntity<Dictionary<string, object>>(
                    "CvProject",
                    r => r.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CvProject__Proje__48CFD27E"),
                    l => l.HasOne<Cv>().WithMany()
                        .HasForeignKey("Cvid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CvProjects__CVID__47DBAE45"),
                    j =>
                    {
                        j.HasKey("Cvid", "ProjectId").HasName("PK__CvProjec__B72D57AE096E4FA5");
                        j.ToTable("CvProjects");
                        j.IndexerProperty<int>("Cvid").HasColumnName("CVID");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("ProjectID");
                    });
        });

        modelBuilder.Entity<CvView>(entity =>
        {
            entity.HasKey(e => e.Cvid).HasName("PK__CvViews__A04CFC43E1BE5B2A");

            entity.Property(e => e.Cvid)
                .ValueGeneratedNever()
                .HasColumnName("CVID");

            entity.HasOne(d => d.Cv).WithOne(p => p.CvView)
                .HasForeignKey<CvView>(d => d.Cvid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CvViews__CVID__4CA06362");
        });

        modelBuilder.Entity<Education>(entity =>
        {
            entity.HasKey(e => e.Eid).HasName("PK__Educatio__C190170B7306CCE1");

            entity.ToTable("Education");

            entity.Property(e => e.Eid).HasColumnName("EID");
            entity.Property(e => e.Cvid).HasColumnName("CVID");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Cv).WithMany(p => p.Educations)
                .HasForeignKey(d => d.Cvid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Education__CVID__52593CB8");

            entity.HasMany(d => d.Sids).WithMany(p => p.Eids)
                .UsingEntity<Dictionary<string, object>>(
                    "EducationSkill",
                    r => r.HasOne<Skill>().WithMany()
                        .HasForeignKey("Sid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EducationSk__SID__5629CD9C"),
                    l => l.HasOne<Education>().WithMany()
                        .HasForeignKey("Eid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EducationSk__EID__5535A963"),
                    j =>
                    {
                        j.HasKey("Eid", "Sid").HasName("PK__Educatio__DD31829C9418B051");
                        j.ToTable("EducationSkills");
                        j.IndexerProperty<int>("Eid").HasColumnName("EID");
                        j.IndexerProperty<int>("Sid").HasColumnName("SID");
                    });
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Mid).HasName("PK__Messages__C797348AC79E9F3C");

            entity.Property(e => e.Mid).HasColumnName("MID");
            entity.Property(e => e.Content).HasMaxLength(300);
            entity.Property(e => e.Reciever).HasMaxLength(450);
            entity.Property(e => e.Sender).HasMaxLength(450);

            entity.HasOne(d => d.RecieverNavigation).WithMany(p => p.MessageRecieverNavigations)
                .HasForeignKey(d => d.Reciever)
                .HasConstraintName("FK__Messages__Reciev__160F4887");

            entity.HasOne(d => d.SenderNavigation).WithMany(p => p.MessageSenderNavigations)
                .HasForeignKey(d => d.Sender)
                .HasConstraintName("FK__Messages__Sender__151B244E");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABED001D3C2A6");

            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Projects)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Projects__Create__17036CC0");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK__Skills__CA195970A1949E5E");

            entity.HasIndex(e => e.Name, "UQ__Skills__737584F63802EA32").IsUnique();

            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
