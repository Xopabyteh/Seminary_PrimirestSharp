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

                    b.ToTable("Foods");
                });

            modelBuilder.Entity("Yearly.Domain.Models.MenuForWeekAgg.MenuForWeek", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("MenusForWeeks", (string)null);
                });

            modelBuilder.Entity("Yearly.Domain.Models.PhotoAgg.Photo", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FoodId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<DateTime>("PublishDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PublisherId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Photos");
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

                    b.ToTable("Users");
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

                            b1.ToTable("FoodPhotoIds", (string)null);

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

                            b1.ToTable("Foods");

                            b1.WithOwner()
                                .HasForeignKey("FoodId");
                        });

                    b.Navigation("PhotoIds");

                    b.Navigation("PrimirestFoodIdentifier")
                        .IsRequired();
                });

            modelBuilder.Entity("Yearly.Domain.Models.MenuForWeekAgg.MenuForWeek", b =>
                {
                    b.OwnsMany("Yearly.Domain.Models.MenuAgg.ValueObjects.MenuForDay", "MenusForDays", b1 =>
                        {
                            b1.Property<int>("PrimirestMenuForWeekId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<DateTime>("Date")
                                .HasColumnType("datetime2");

                            b1.HasKey("PrimirestMenuForWeekId", "Id");

                            b1.ToTable("MenusForDays", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("PrimirestMenuForWeekId");

                            b1.OwnsMany("Yearly.Domain.Models.FoodAgg.ValueObjects.FoodId", "FoodIds", b2 =>
                                {
                                    b2.Property<Guid>("Value")
                                        .HasColumnType("uniqueidentifier")
                                        .HasColumnName("FoodId");

                                    b2.Property<int>("MenuForDayId")
                                        .HasColumnType("int");

                                    b2.Property<int>("MenuForDayPrimirestMenuForWeekId")
                                        .HasColumnType("int");

                                    b2.HasKey("Value");

                                    b2.HasIndex("MenuForDayPrimirestMenuForWeekId", "MenuForDayId");

                                    b2.ToTable("MenuFoodIds", (string)null);

                                    b2.WithOwner()
                                        .HasForeignKey("MenuForDayPrimirestMenuForWeekId", "MenuForDayId");
                                });

                            b1.Navigation("FoodIds");
                        });

                    b.Navigation("MenusForDays");
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

                            b1.ToTable("UserPhotoIds", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsMany("Yearly.Domain.Models.UserAgg.ValueObjects.UserRole", "Roles", b1 =>
                        {
                            b1.Property<int>("UserId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<string>("RoleCode")
                                .IsRequired()
                                .HasMaxLength(3)
                                .HasColumnType("nvarchar(3)");

                            b1.HasKey("UserId", "Id");

                            b1.ToTable("UserRoles", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("PhotoIds");

                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}
