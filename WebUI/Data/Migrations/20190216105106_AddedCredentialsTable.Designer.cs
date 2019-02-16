﻿// <auto-generated />
using EaisApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using WebUI.Data;
using WebUI.Models;

namespace WebUI.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20190216105106_AddedCredentialsTable")]
    partial class AddedCredentialsTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("WebUI.Data.Entities.DiagnosticCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AllowedMaxWeight")
                        .IsRequired();

                    b.Property<string>("BodyNumber")
                        .HasMaxLength(50);

                    b.Property<int?>("BrakeType")
                        .IsRequired();

                    b.Property<string>("CardId")
                        .HasMaxLength(30);

                    b.Property<int>("CardType");

                    b.Property<int?>("Category")
                        .IsRequired();

                    b.Property<int?>("CategoryCommon")
                        .IsRequired();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<bool>("Deleted");

                    b.Property<DateTime?>("DocumentIssueDate")
                        .IsRequired();

                    b.Property<string>("DocumentIssuer")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("DocumentNumber")
                        .IsRequired()
                        .HasMaxLength(6);

                    b.Property<string>("DocumentSeries")
                        .IsRequired()
                        .HasMaxLength(4);

                    b.Property<int?>("DocumentType")
                        .IsRequired();

                    b.Property<DateTime?>("ExpirationDate")
                        .IsRequired();

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("FrameNumber")
                        .HasMaxLength(50);

                    b.Property<int?>("FuelType")
                        .IsRequired();

                    b.Property<bool>("IsForeigner");

                    b.Property<int?>("IssueYear")
                        .IsRequired();

                    b.Property<string>("Lastname")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Manufacturer")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Note")
                        .HasMaxLength(250);

                    b.Property<string>("Patronymic")
                        .HasMaxLength(30);

                    b.Property<string>("RegNumber")
                        .HasMaxLength(10);

                    b.Property<DateTime?>("RegisteredDate");

                    b.Property<int?>("Running")
                        .IsRequired();

                    b.Property<string>("TyreManufacturer")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("UserId");

                    b.Property<string>("VIN")
                        .HasMaxLength(17);

                    b.Property<int?>("Weight")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BodyNumber");

                    b.HasIndex("DocumentNumber");

                    b.HasIndex("DocumentSeries");

                    b.HasIndex("FrameNumber");

                    b.HasIndex("RegNumber");

                    b.HasIndex("UserId");

                    b.HasIndex("VIN");

                    b.ToTable("DiagnosticCards");
                });

            modelBuilder.Entity("WebUI.Data.Entities.EaistoCredential", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Login")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("EaistoCredentials");
                });

            modelBuilder.Entity("WebUI.Data.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool?>("IsApproved");

                    b.Property<bool>("IsRemoved");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("WebUI.Data.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("WebUI.Data.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebUI.Data.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("WebUI.Data.Entities.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WebUI.Data.Entities.DiagnosticCard", b =>
                {
                    b.HasOne("WebUI.Data.Entities.User", "User")
                        .WithMany("DiagnosticCards")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
