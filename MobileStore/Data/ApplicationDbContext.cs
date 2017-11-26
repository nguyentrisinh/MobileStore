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
