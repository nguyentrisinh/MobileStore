using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MobileStore.Models;

namespace MobileStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.Entity<Item>().HasKey(ent => ent.ItemID);
            //builder.Entity<Item>().HasOne(ent => ent.ModelFromSupplier).WithMany(ent => ent.Items).HasForeignKey(ent => ent.ModelFromSupplierID)
            //    .IsRequired().OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<ModelFromSupplier>().HasKey(ent => ent.ModelFromSupplierID);

            builder.Entity<ApplicationUser>().HasKey(ent => ent.Id);
            builder.Entity<Brand>().HasKey(ent => ent.BrandID);
            builder.Entity<Customer>().HasKey(ent => ent.CustomerID);
            builder.Entity<Item>().HasKey(ent => ent.ItemID);
            builder.Entity<Model>().HasKey(ent => ent.ModelID);
            builder.Entity<ModelFromSupplier>().HasKey(ent => ent.ModelFromSupplierID);
            builder.Entity<Order>().HasKey(ent => ent.OrderID);
            builder.Entity<OrderDetail>().HasKey(ent => ent.OrderDetailID);
            builder.Entity<ReturnItem>().HasKey(ent => ent.ReturnItemID);
            builder.Entity<Supplier>().HasKey(ent => ent.SupplierID);
            builder.Entity<WarrantyCard>().HasKey(ent => ent.WarrantyCardID);
            builder.Entity<WarrantyDetail>().HasKey(ent => ent.WarrantyDetailID);

            builder.Entity<Item>().HasOne(ent => ent.ModelFromSupplier).WithMany(ent => ent.Items).HasForeignKey(ent => ent.ModelFromSupplierID)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Item>().HasOne(ent => ent.Model).WithMany(ent => ent.Items).HasForeignKey(ent => ent.ModelID)
                .IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Item>().HasOne(ent => ent.OrderDetail).WithOne(ent => ent.Item).HasForeignKey<OrderDetail>(ent => ent.ItemID)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Item>().HasOne(ent => ent.NewItem).WithOne(ent => ent.NewItem).HasForeignKey<ReturnItem>(ent => ent.NewItemID)
                .IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Item>().HasOne(ent => ent.OldItem).WithOne(ent => ent.OldItem).HasForeignKey<ReturnItem>(ent => ent.OldItemID)
                .IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Model>().HasOne(ent => ent.Brand).WithMany(ent => ent.Models).HasForeignKey(ent => ent.BrandID)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ModelFromSupplier>().HasOne(ent => ent.Supplier).WithMany(ent => ent.ModelFromSuppliers).HasForeignKey(ent => ent.SupplierID)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ModelFromSupplier>().HasOne(ent => ent.Model).WithMany(ent => ent.ModelFromSuppliers).HasForeignKey(ent => ent.ModelID)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Order>().HasOne(ent => ent.ApplicationUser).WithMany(ent => ent.Orders).HasForeignKey(ent => ent.ApplicationUserID)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Order>().HasOne(ent => ent.Customer).WithMany(ent => ent.Orders).HasForeignKey(ent => ent.CustomerID)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderDetail>().HasOne(ent => ent.Order).WithMany(ent => ent.OrderDetails).HasForeignKey(ent => ent.OrderID)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WarrantyCard>().HasOne(ent => ent.Item).WithMany(ent => ent.WarrantyCards).HasForeignKey(ent => ent.ItemID)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WarrantyDetail>().HasOne(ent => ent.WarrantyCard).WithMany(ent => ent.WarrantyDetails).HasForeignKey(ent => ent.WarrantyCardID)
                .IsRequired().OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<MobileStore.Models.ApplicationUser> ApplicationUser { get; set; }
        public DbSet<MobileStore.Models.Brand> Brand { get; set; }
        public DbSet<MobileStore.Models.Customer> Customer { get; set; }
        public DbSet<MobileStore.Models.Item> Item { get; set; }
        public DbSet<MobileStore.Models.Model> Model { get; set; }
        public DbSet<MobileStore.Models.ModelFromSupplier> ModelFromSupplier { get; set; }
        public DbSet<MobileStore.Models.Order> Order { get; set; }
        public DbSet<MobileStore.Models.OrderDetail> OrderDetail { get; set; }
        public DbSet<MobileStore.Models.ReturnItem> ReturnItem { get; set; }
        public DbSet<MobileStore.Models.Supplier> Supplier { get; set; }
        public DbSet<MobileStore.Models.WarrantyCard> WarrantyCard { get; set; }
        public DbSet<MobileStore.Models.WarrantyDetail> WarrantyDetail { get; set; }
    }
}
