﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using cvprojekt.Models;

#nullable disable

namespace cvprojekt.Migrations
{
    [DbContext(typeof(CvDbContext))]
    [Migration("20241223120313_TestMigration")]
    partial class TestMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CvProject", b =>
                {
                    b.Property<int>("Cvid")
                        .HasColumnType("int")
                        .HasColumnName("CVID");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int")
                        .HasColumnName("ProjectID");

                    b.HasKey("Cvid", "ProjectId")
                        .HasName("PK__CvProjec__B72D57AE096E4FA5");

                    b.HasIndex("ProjectId");

                    b.ToTable("CvProjects", (string)null);
                });

            modelBuilder.Entity("EducationSkill", b =>
                {
                    b.Property<int>("Eid")
                        .HasColumnType("int")
                        .HasColumnName("EID");

                    b.Property<int>("Sid")
                        .HasColumnType("int")
                        .HasColumnName("SID");

                    b.HasKey("Eid", "Sid")
                        .HasName("PK__Educatio__DD31829C9418B051");

                    b.HasIndex("Sid");

                    b.ToTable("EducationSkills", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("cvprojekt.Models.Cv", b =>
                {
                    b.Property<int>("Cvid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CVID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Cvid"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Owner")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Cvid")
                        .HasName("PK__CV__A04CFC43EF4B5070");

                    b.HasIndex("Owner");

                    b.ToTable("CV", (string)null);
                });

            modelBuilder.Entity("cvprojekt.Models.CvView", b =>
                {
                    b.Property<int>("Cvid")
                        .HasColumnType("int")
                        .HasColumnName("CVID");

                    b.Property<int>("ViewCount")
                        .HasColumnType("int");

                    b.HasKey("Cvid")
                        .HasName("PK__CvViews__A04CFC43E1BE5B2A");

                    b.ToTable("CvViews");
                });

            modelBuilder.Entity("cvprojekt.Models.Education", b =>
                {
                    b.Property<int>("Eid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("EID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Eid"));

                    b.Property<int>("Cvid")
                        .HasColumnType("int")
                        .HasColumnName("CVID");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Eid")
                        .HasName("PK__Educatio__C190170B7306CCE1");

                    b.HasIndex("Cvid");

                    b.ToTable("Education", (string)null);
                });

            modelBuilder.Entity("cvprojekt.Models.Message", b =>
                {
                    b.Property<int>("Mid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("MID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Mid"));

                    b.Property<string>("Content")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<bool?>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Reciever")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Sender")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateOnly?>("TimeSent")
                        .HasColumnType("date");

                    b.HasKey("Mid")
                        .HasName("PK__Messages__C797348AC79E9F3C");

                    b.HasIndex("Reciever");

                    b.HasIndex("Sender");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("cvprojekt.Models.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ProjectID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProjectId"));

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("ProjectId")
                        .HasName("PK__Projects__761ABED001D3C2A6");

                    b.HasIndex("CreatedBy");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("cvprojekt.Models.Skill", b =>
                {
                    b.Property<int>("Sid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("SID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Sid"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Sid")
                        .HasName("PK__Skills__CA195970A1949E5E");

                    b.HasIndex(new[] { "Name" }, "UQ__Skills__737584F63802EA32")
                        .IsUnique();

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("cvprojekt.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("Id");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<byte[]>("ProfilePicture")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varbinary(100)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id")
                        .HasName("PK__Users__1788CCAC2E013538");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex(new[] { "Email" }, "UQ__Users__A9D10534D5022A46")
                        .IsUnique()
                        .HasFilter("[Email] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("CvProject", b =>
                {
                    b.HasOne("cvprojekt.Models.Cv", null)
                        .WithMany()
                        .HasForeignKey("Cvid")
                        .IsRequired()
                        .HasConstraintName("FK__CvProjects__CVID__47DBAE45");

                    b.HasOne("cvprojekt.Models.Project", null)
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .IsRequired()
                        .HasConstraintName("FK__CvProject__Proje__48CFD27E");
                });

            modelBuilder.Entity("EducationSkill", b =>
                {
                    b.HasOne("cvprojekt.Models.Education", null)
                        .WithMany()
                        .HasForeignKey("Eid")
                        .IsRequired()
                        .HasConstraintName("FK__EducationSk__EID__5535A963");

                    b.HasOne("cvprojekt.Models.Skill", null)
                        .WithMany()
                        .HasForeignKey("Sid")
                        .IsRequired()
                        .HasConstraintName("FK__EducationSk__SID__5629CD9C");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("cvprojekt.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("cvprojekt.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("cvprojekt.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("cvprojekt.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("cvprojekt.Models.Cv", b =>
                {
                    b.HasOne("cvprojekt.Models.User", "OwnerNavigation")
                        .WithMany("Cvs")
                        .HasForeignKey("Owner")
                        .HasConstraintName("FK__CV__Owner__44FF419A");

                    b.Navigation("OwnerNavigation");
                });

            modelBuilder.Entity("cvprojekt.Models.CvView", b =>
                {
                    b.HasOne("cvprojekt.Models.Cv", "Cv")
                        .WithOne("CvView")
                        .HasForeignKey("cvprojekt.Models.CvView", "Cvid")
                        .IsRequired()
                        .HasConstraintName("FK__CvViews__CVID__4CA06362");

                    b.Navigation("Cv");
                });

            modelBuilder.Entity("cvprojekt.Models.Education", b =>
                {
                    b.HasOne("cvprojekt.Models.Cv", "Cv")
                        .WithMany("Educations")
                        .HasForeignKey("Cvid")
                        .IsRequired()
                        .HasConstraintName("FK__Education__CVID__52593CB8");

                    b.Navigation("Cv");
                });

            modelBuilder.Entity("cvprojekt.Models.Message", b =>
                {
                    b.HasOne("cvprojekt.Models.User", "RecieverNavigation")
                        .WithMany("MessageRecieverNavigations")
                        .HasForeignKey("Reciever")
                        .HasConstraintName("FK__Messages__Reciev__59FA5E80");

                    b.HasOne("cvprojekt.Models.User", "SenderNavigation")
                        .WithMany("MessageSenderNavigations")
                        .HasForeignKey("Sender")
                        .HasConstraintName("FK__Messages__Sender__59063A47");

                    b.Navigation("RecieverNavigation");

                    b.Navigation("SenderNavigation");
                });

            modelBuilder.Entity("cvprojekt.Models.Project", b =>
                {
                    b.HasOne("cvprojekt.Models.User", "CreatedByNavigation")
                        .WithMany("ProjectsNavigation")
                        .HasForeignKey("CreatedBy")
                        .HasConstraintName("FK__Projects__Create__3E52440B");

                    b.Navigation("CreatedByNavigation");
                });

            modelBuilder.Entity("cvprojekt.Models.Cv", b =>
                {
                    b.Navigation("CvView");

                    b.Navigation("Educations");
                });

            modelBuilder.Entity("cvprojekt.Models.User", b =>
                {
                    b.Navigation("Cvs");

                    b.Navigation("MessageRecieverNavigations");

                    b.Navigation("MessageSenderNavigations");

                    b.Navigation("ProjectsNavigation");
                });
#pragma warning restore 612, 618
        }
    }
}
