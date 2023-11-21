﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Yearly.Infrastructure.Persistence;

#nullable disable

namespace Yearly.Infrastructure.Migrations
{
    [DbContext(typeof(PrimirestSharpDbContext))]
    partial class PrimirestSharpDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Yearly.Domain.Models.FoodAgg.Food", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AliasForFoodId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Allergens")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.ToTable("Foods", "Domain");
                });

            modelBuilder.Entity("Yearly.Domain.Models.FoodAgg.ValueObjects.FoodSimilarityRecord", b =>
                {
                    b.Property<Guid>("NewlyPersistedFoodId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PotentialAliasOriginId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Similarity")
                        .HasColumnType("float");

                    b.HasKey("NewlyPersistedFoodId", "PotentialAliasOriginId");

                    b.ToTable("FoodSimilarities", "Domain");
                });

            modelBuilder.Entity("Yearly.Domain.Models.PhotoAgg.Photo", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FoodId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("bit");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<DateTime>("PublishDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PublisherId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Photos", "Domain");
                });

            modelBuilder.Entity("Yearly.Domain.Models.UserAgg.User", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Users", "Domain");
                });

            modelBuilder.Entity("Yearly.Domain.Models.WeeklyMenuAgg.WeeklyMenu", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("WeeklyMenus", "Domain");
                });

            modelBuilder.Entity("Yearly.Domain.Models.FoodAgg.Food", b =>
                {
                    b.OwnsMany("Yearly.Domain.Models.PhotoAgg.ValueObjects.PhotoId", "PhotoIds", b1 =>
                        {
                            b1.Property<Guid>("Value")
                                .HasColumnType("uniqueidentifier")
                                .HasColumnName("PhotoId");

                            b1.Property<Guid>("FoodId")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("Value");

                            b1.HasIndex("FoodId");

                            b1.ToTable("FoodPhotoIds", "Domain");

                            b1.WithOwner()
                                .HasForeignKey("FoodId");
                        });

                    b.OwnsOne("Yearly.Domain.Models.FoodAgg.ValueObjects.PrimirestFoodIdentifier", "PrimirestFoodIdentifier", b1 =>
                        {
                            b1.Property<Guid>("FoodId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("DayId")
                                .HasColumnType("int")
                                .HasColumnName("PrimirestDayId");

                            b1.Property<int>("ItemId")
                                .HasColumnType("int")
                                .HasColumnName("PrimirestItemId");

                            b1.Property<int>("MenuId")
                                .HasColumnType("int")
                                .HasColumnName("PrimirestMenuId");

                            b1.HasKey("FoodId");

                            b1.ToTable("Foods", "Domain");

                            b1.WithOwner()
                                .HasForeignKey("FoodId");
                        });

                    b.Navigation("PhotoIds");

                    b.Navigation("PrimirestFoodIdentifier")
                        .IsRequired();
                });

            modelBuilder.Entity("Yearly.Domain.Models.UserAgg.User", b =>
                {
                    b.OwnsMany("Yearly.Domain.Models.PhotoAgg.ValueObjects.PhotoId", "PhotoIds", b1 =>
                        {
                            b1.Property<int>("UserId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<Guid>("Value")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("UserId", "Id");

                            b1.ToTable("UserPhotoIds", "Domain");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsMany("Yearly.Domain.Models.UserAgg.ValueObjects.UserRole", "Roles", b1 =>
                        {
                            b1.Property<string>("RoleCode")
                                .HasMaxLength(3)
                                .HasColumnType("nvarchar(3)");

                            b1.Property<int>("UserId")
                                .HasColumnType("int");

                            b1.HasKey("RoleCode");

                            b1.HasIndex("UserId");

                            b1.ToTable("UserRoles", "Domain");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("PhotoIds");

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("Yearly.Domain.Models.WeeklyMenuAgg.WeeklyMenu", b =>
                {
                    b.OwnsMany("Yearly.Domain.Models.MenuAgg.ValueObjects.DailyMenu", "DailyMenus", b1 =>
                        {
                            b1.Property<int>("WeeklyMenuId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<DateTime>("Date")
                                .HasColumnType("datetime2");

                            b1.HasKey("WeeklyMenuId", "Id");

                            b1.ToTable("DailyMenus", "Domain");

                            b1.WithOwner()
                                .HasForeignKey("WeeklyMenuId");

                            b1.OwnsMany("Yearly.Domain.Models.MenuAgg.ValueObjects.MenuFood", "Foods", b2 =>
                                {
                                    b2.Property<Guid>("FoodId")
                                        .HasColumnType("uniqueidentifier")
                                        .HasColumnName("FoodId");

                                    b2.Property<int>("DailyMenuId")
                                        .HasColumnType("int");

                                    b2.Property<int>("DailyMenuWeeklyMenuId")
                                        .HasColumnType("int");

                                    b2.HasKey("FoodId");

                                    b2.HasIndex("DailyMenuWeeklyMenuId", "DailyMenuId");

                                    b2.ToTable("MenuFoodIds", "Domain");

                                    b2.WithOwner()
                                        .HasForeignKey("DailyMenuWeeklyMenuId", "DailyMenuId");
                                });

                            b1.Navigation("Foods");
                        });

                    b.Navigation("DailyMenus");
                });
#pragma warning restore 612, 618
        }
    }
}
