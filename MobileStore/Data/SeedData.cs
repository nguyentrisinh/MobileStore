#define SeedOnly
#if SeedOnly

using MobileStore.Authorization;
using MobileStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using MobileStore.Data;
using MobileStore.Models;

namespace ContactManager.Data
{
    public static class SeedData
    {
        #region snippet_Initialize
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes we are seeding 2 users both with the same password.
                // The password is set with the following command:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@ood.com");
                await EnsureRole(serviceProvider, adminID, Constants.AdminRole);

                // allowed user can create and edit contacts that they create
                var saleID = await EnsureUser(serviceProvider, testUserPw, "sale@ood.com");
                await EnsureRole(serviceProvider, saleID, Constants.SaleRole);

                var warehousemanagerID = await EnsureUser(serviceProvider, testUserPw, "warehousemanager@ood.com");
                await EnsureRole(serviceProvider, warehousemanagerID, Constants.WarehouseManagerRole);

                var technicalID = await EnsureUser(serviceProvider, testUserPw, "technical@ood.com");
                await EnsureRole(serviceProvider, technicalID, Constants.TechnicalRole);




                //SeedDB(context);
                //SeedDBBrand(context);
                //SeedDBModel(context);
            }
        }
    
        #endregion

        //private static void SeedDB(ApplicationDbContext context)
        //{
        //    if (context.Constant.Any())
        //    {
        //        return;   // DB has been seeded
        //    }
        //    context.Constant.AddRange(
        //        new Constant
        //        {
        //           ConstantID = 1,
        //           Name = "Thời hạn đổi trả",
        //           Parameter = 10,
        //           Description = "Đơn vị ngày"
        //        }
        //    );

        //    context.SaveChanges();
        //}

        //private static void SeedDBBrand(ApplicationDbContext context)
        //{
        //    if (context.Brand.Any())
        //    {
        //        return;   // DB has been seeded
        //    }
        //    context.Brand.AddRange(
        //        new Brand
        //        {
        //             BrandID= 1,
        //            Country = "Hàn Quốc",
        //            Description = "",
        //            Name = "Samsung",
        //        }
        //    );

        //    context.SaveChanges();
        //}

        //private static void SeedDBModel(ApplicationDbContext context)
        //{
        //    if (context.Model.Any())
        //    {
        //        return;   // DB has been seeded
        //    }
        //    context.Model.AddRange(
        //        new Model
        //        {
        //             ModelID = 1,
        //            BrandID = 1,
        //            Description = "",
        //            Name = "Samsung Galaxy S8 Gold",
        //            Color = "Gold",
        //            Specification = "",
        //            Type = ModelType.Device,
        //        }
        //    );

        //    context.SaveChanges();
        //}

        #region snippet_CreateRoles        

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new ApplicationUser { UserName = UserName };
                await userManager.CreateAsync(user, testUserPw);
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(uid);

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
        #endregion
    }
}
#endif