using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace cvprojekt.Models;

public partial class CvDbContext : IdentityDbContext<User>
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

    public virtual DbSet<Education> Educations { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<User> Users { get; set; }


    /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("DefaultConnection");*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cv>(entity =>
        {
            entity.HasKey(e => e.Cvid).HasName("PK__CV__A04CFC43EF4B5070");

            entity.ToTable("CV");

            entity.Property(e => e.Cvid).HasColumnName("CVID");

            entity.HasOne(d => d.OwnerNavigation).WithMany(p => p.Cvs)
                .HasForeignKey(d => d.Owner)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CV__Owner__44FF419A");

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

            entity.HasOne(d => d.RecieverNavigation).WithMany(p => p.MessageRecieverNavigations)
                .HasForeignKey(d => d.Reciever)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__Reciev__59FA5E80");

            entity.HasOne(d => d.SenderNavigation).WithMany(p => p.MessageSenderNavigations)
                .HasForeignKey(d => d.Sender)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__Sender__59063A47");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABED001D3C2A6");

            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Projects)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Projects__Create__3E52440B");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK__Skills__CA195970A1949E5E");

            entity.HasIndex(e => e.Name, "UQ__Skills__737584F63802EA32").IsUnique();

            entity.Property(e => e.Sid).HasColumnName("SID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__1788CCAC2E013538");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534D5022A46").IsUnique();

            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(100);
            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(100)
                .HasDefaultValue("ProfilePictureURL");

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
