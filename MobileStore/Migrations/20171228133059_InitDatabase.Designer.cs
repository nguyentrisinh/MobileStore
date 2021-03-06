﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using MobileStore.Data;
using MobileStore.Models;
using System;

namespace MobileStore.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20171228133059_InitDatabase")]
    partial class InitDatabase
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

            modelBuilder.Entity("MobileStore.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("Address");

                    b.Property<string>("AvatarUrl");

                    b.Property<DateTime>("Birthday");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("Phone");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<int>("Role");

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

            modelBuilder.Entity("MobileStore.Models.Brand", b =>
                {
                    b.Property<int>("BrandID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Country");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.HasKey("BrandID");

                    b.ToTable("Brand");
                });

            modelBuilder.Entity("MobileStore.Models.Constant", b =>
                {
                    b.Property<int>("ConstantID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<int>("Parameter");

                    b.HasKey("ConstantID");

                    b.ToTable("Constant");
                });

            modelBuilder.Entity("MobileStore.Models.Customer", b =>
                {
                    b.Property<int>("CustomerID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<DateTime>("Birthday");

                    b.Property<string>("Country");

                    b.Property<int>("Gender");

                    b.Property<string>("Name");

                    b.Property<string>("Phone");

                    b.HasKey("CustomerID");

                    b.ToTable("Customer");
                });

            modelBuilder.Entity("MobileStore.Models.Item", b =>
                {
                    b.Property<int>("ItemID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("IMEI");

                    b.Property<int>("ModelFromSupplierID");

                    b.Property<int>("ModelID");

                    b.Property<string>("Name");

                    b.Property<string>("Note");

                    b.Property<string>("SerializerNumber");

                    b.Property<int>("Status");

                    b.HasKey("ItemID");

                    b.HasIndex("ModelFromSupplierID");

                    b.HasIndex("ModelID");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("MobileStore.Models.Model", b =>
                {
                    b.Property<int>("ModelID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BrandID");

                    b.Property<string>("Color");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<string>("PictureOneUrl");

                    b.Property<string>("PictureThreeUrl");

                    b.Property<string>("PictureTwoUrl");

                    b.Property<string>("Specification");

                    b.Property<int>("Type");

                    b.HasKey("ModelID");

                    b.HasIndex("BrandID");

                    b.ToTable("Model");
                });

            modelBuilder.Entity("MobileStore.Models.ModelFromSupplier", b =>
                {
                    b.Property<int>("ModelFromSupplierID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<int>("ModelID");

                    b.Property<int>("Period");

                    b.Property<double>("PriceBought");

                    b.Property<double>("PriceSold");

                    b.Property<int>("Quantity");

                    b.Property<Guid>("StockReceivingID");

                    b.HasKey("ModelFromSupplierID");

                    b.HasIndex("ModelID");

                    b.HasIndex("StockReceivingID");

                    b.ToTable("ModelFromSupplier");
                });

            modelBuilder.Entity("MobileStore.Models.Order", b =>
                {
                    b.Property<int>("OrderID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationUserID")
                        .IsRequired();

                    b.Property<int>("CustomerID");

                    b.Property<DateTime>("Date");

                    b.Property<bool>("IsPrinted");

                    b.Property<double>("Total");

                    b.HasKey("OrderID");

                    b.HasIndex("ApplicationUserID");

                    b.HasIndex("CustomerID");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("MobileStore.Models.OrderDetail", b =>
                {
                    b.Property<int>("OrderDetailID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ItemID");

                    b.Property<int>("OrderID");

                    b.Property<double>("PriceSold");

                    b.HasKey("OrderDetailID");

                    b.HasIndex("ItemID")
                        .IsUnique();

                    b.HasIndex("OrderID");

                    b.ToTable("OrderDetail");
                });

            modelBuilder.Entity("MobileStore.Models.ReturnItem", b =>
                {
                    b.Property<int>("ReturnItemID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationUserID")
                        .IsRequired();

                    b.Property<string>("DefectInfo");

                    b.Property<int>("NewItemID");

                    b.Property<int>("OldItemID");

                    b.Property<DateTime>("ReturnDate");

                    b.HasKey("ReturnItemID");

                    b.HasIndex("ApplicationUserID");

                    b.HasIndex("NewItemID")
                        .IsUnique();

                    b.HasIndex("OldItemID")
                        .IsUnique();

                    b.ToTable("ReturnItem");
                });

            modelBuilder.Entity("MobileStore.Models.StockReceiving", b =>
                {
                    b.Property<Guid>("StockReceivingID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationUserID")
                        .IsRequired();

                    b.Property<DateTime>("Date");

                    b.Property<int>("SupplierID");

                    b.HasKey("StockReceivingID");

                    b.HasIndex("ApplicationUserID");

                    b.HasIndex("SupplierID");

                    b.ToTable("StockReceiving");
                });

            modelBuilder.Entity("MobileStore.Models.Supplier", b =>
                {
                    b.Property<int>("SupplierID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("Code");

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.Property<string>("Phone");

                    b.Property<string>("PicEmail");

                    b.Property<string>("PicName");

                    b.Property<string>("PicPhone");

                    b.Property<int>("Status");

                    b.HasKey("SupplierID");

                    b.ToTable("Supplier");
                });

            modelBuilder.Entity("MobileStore.Models.WarrantyCard", b =>
                {
                    b.Property<int>("WarrantyCardID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationUserID")
                        .IsRequired();

                    b.Property<DateTime>("EndDate");

                    b.Property<bool>("IsDisabled");

                    b.Property<bool>("IsPrinted");

                    b.Property<int>("ItemID");

                    b.Property<int>("NumberOfWarranty");

                    b.Property<DateTime>("StartDate");

                    b.Property<Guid>("TransactionCode");

                    b.HasKey("WarrantyCardID");

                    b.HasIndex("ApplicationUserID");

                    b.HasIndex("ItemID");

                    b.ToTable("WarrantyCard");
                });

            modelBuilder.Entity("MobileStore.Models.WarrantyDetail", b =>
                {
                    b.Property<int>("WarrantyDetailID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationUserID")
                        .IsRequired();

                    b.Property<DateTime>("Date");

                    b.Property<string>("DefectInfo");

                    b.Property<DateTime>("ExpectedDate");

                    b.Property<bool>("IsPrinted");

                    b.Property<DateTime?>("ReturnedDate");

                    b.Property<int>("Status");

                    b.Property<int>("WarrantyCardID");

                    b.Property<DateTime?>("WarrantyDate");

                    b.HasKey("WarrantyDetailID");

                    b.HasIndex("ApplicationUserID");

                    b.HasIndex("WarrantyCardID");

                    b.ToTable("WarrantyDetail");
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
                    b.HasOne("MobileStore.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MobileStore.Models.ApplicationUser")
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

                    b.HasOne("MobileStore.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("MobileStore.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileStore.Models.Item", b =>
                {
                    b.HasOne("MobileStore.Models.ModelFromSupplier", "ModelFromSupplier")
                        .WithMany("Items")
                        .HasForeignKey("ModelFromSupplierID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileStore.Models.Model", "Model")
                        .WithMany("Items")
                        .HasForeignKey("ModelID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("MobileStore.Models.Model", b =>
                {
                    b.HasOne("MobileStore.Models.Brand", "Brand")
                        .WithMany("Models")
                        .HasForeignKey("BrandID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileStore.Models.ModelFromSupplier", b =>
                {
                    b.HasOne("MobileStore.Models.Model", "Model")
                        .WithMany("ModelFromSuppliers")
                        .HasForeignKey("ModelID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileStore.Models.StockReceiving", "StockReceiving")
                        .WithMany("ModelFromSuppliers")
                        .HasForeignKey("StockReceivingID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileStore.Models.Order", b =>
                {
                    b.HasOne("MobileStore.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("Orders")
                        .HasForeignKey("ApplicationUserID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileStore.Models.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileStore.Models.OrderDetail", b =>
                {
                    b.HasOne("MobileStore.Models.Item", "Item")
                        .WithOne("OrderDetail")
                        .HasForeignKey("MobileStore.Models.OrderDetail", "ItemID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MobileStore.Models.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileStore.Models.ReturnItem", b =>
                {
                    b.HasOne("MobileStore.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("ReturnItems")
                        .HasForeignKey("ApplicationUserID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MobileStore.Models.Item", "NewItem")
                        .WithOne("NewItem")
                        .HasForeignKey("MobileStore.Models.ReturnItem", "NewItemID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MobileStore.Models.Item", "OldItem")
                        .WithOne("OldItem")
                        .HasForeignKey("MobileStore.Models.ReturnItem", "OldItemID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("MobileStore.Models.StockReceiving", b =>
                {
                    b.HasOne("MobileStore.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("StoceReceivings")
                        .HasForeignKey("ApplicationUserID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MobileStore.Models.Supplier", "Supplier")
                        .WithMany("StockReceivings")
                        .HasForeignKey("SupplierID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileStore.Models.WarrantyCard", b =>
                {
                    b.HasOne("MobileStore.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("WarrantyCards")
                        .HasForeignKey("ApplicationUserID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MobileStore.Models.Item", "Item")
                        .WithMany("WarrantyCards")
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MobileStore.Models.WarrantyDetail", b =>
                {
                    b.HasOne("MobileStore.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("WarrantyDetails")
                        .HasForeignKey("ApplicationUserID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MobileStore.Models.WarrantyCard", "WarrantyCard")
                        .WithMany("WarrantyDetails")
                        .HasForeignKey("WarrantyCardID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
