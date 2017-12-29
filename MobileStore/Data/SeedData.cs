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
using System.Collections.Generic;

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

                //var listApplicationUserID = await InitApplicationUser(serviceProvider, testUserPw);

                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@ood.com");
                await EnsureRole(serviceProvider, adminID, Constants.AdminRole);

                // allowed user can create and edit contacts that they create
                var saleID = await EnsureUser(serviceProvider, testUserPw, "sale@ood.com");
                await EnsureRole(serviceProvider, saleID, Constants.SaleRole);

                var warehousemanagerID = await EnsureUser(serviceProvider, testUserPw, "warehousemanager@ood.com");
                await EnsureRole(serviceProvider, warehousemanagerID, Constants.WarehouseManagerRole);

                var technicalID = await EnsureUser(serviceProvider, testUserPw, "technical@ood.com");
                await EnsureRole(serviceProvider, technicalID, Constants.TechnicalRole);

                var adminTwoID = await EnsureUser(serviceProvider, "Abc123456!", "admintwo@gmail.com");
                await EnsureRole(serviceProvider, adminTwoID, Constants.AdminRole);

                // Add Full info Admin user
                var employerToanAdmin = new ApplicationUser
                {
                    UserName = "toannguyen@gmail.com",
                    Email = "toannguyen@gmail.com",
                    FirstName = "Toan",
                    LastName = "Nguyen Thanh Thien",
                    Birthday = DateTime.Parse("1992-09-20"),
                    Phone = "01672699288",
                    PhoneNumber = "01672699288",
                    Address = "30/15 Đặng Văn Bi, Gò Vấp",
                    Role = UserRole.Admin
                };
                employerToanAdmin = await CreateApplicationUser(serviceProvider, employerToanAdmin, "Abc123456!");
                await EnsureRole(serviceProvider, employerToanAdmin.Id, Constants.AdminRole);

                // Add Full info Sale user
                var employerThuSale = new ApplicationUser
                {
                    UserName = "thunguyen@gmail.com",
                    Email = "thunguyen@gmail.com",
                    FirstName = "Thư",
                    LastName = "Châu Hồng Anh",
                    Birthday = DateTime.Parse("1996-03-16"),
                    Phone = "0168794582",
                    PhoneNumber = "0168794582",
                    Address = "80 Cách Mạng Tháng 8, Tân Bình",
                    Role = UserRole.Sale
                };
                employerThuSale = await CreateApplicationUser(serviceProvider, employerThuSale, "Abc123456!");
                await EnsureRole(serviceProvider, employerThuSale.Id, Constants.SaleRole);

                // Add Full info WarehouseManager user
                var employerSinhWarehouse = new ApplicationUser
                {
                    UserName = "trisinh1996@gmail.com",
                    Email = "trisinh1996@gmail.com",
                    FirstName = "Sinh",
                    LastName = "Nguyễn Tri",
                    Birthday = DateTime.Parse("1996-08-10"),
                    Phone = "01672699288",
                    PhoneNumber = "01672699288",
                    Address = "162 Huỳnh Văn Bánh, Quận Phú Nhuận",
                    Role = UserRole.WarehouseManager
                };
                employerSinhWarehouse = await CreateApplicationUser(serviceProvider, employerSinhWarehouse, "Abc123456!");
                await EnsureRole(serviceProvider, employerSinhWarehouse.Id, Constants.WarehouseManagerRole);

                // Add Full info Technical user
                var employerNgaTechnical = new ApplicationUser
                {
                    UserName = "ngavtt@gmail.com",
                    Email = "ngavtt@gmail.com",
                    FirstName = "Nga",
                    LastName = "Võ Thị Thúy",
                    Birthday = DateTime.Parse("1996-10-05"),
                    Phone = "0983254435",
                    PhoneNumber = "0983254435",
                    Address = "ktx khu B Dĩ An, tỉnh Bình Dương",
                    Role = UserRole.Technical
                };
                employerNgaTechnical = await CreateApplicationUser(serviceProvider, employerNgaTechnical, "Abc123456!");
                await EnsureRole(serviceProvider, employerNgaTechnical.Id, Constants.TechnicalRole);

                // Add Full info Accountant user
                var employerPhuongAccountant = new ApplicationUser
                {
                    UserName = "phuongnguyen@gmail.com",
                    Email = "phuongnguyen@gmail.com",
                    FirstName = "Phương",
                    LastName = "Nguyễn Lan",
                    Birthday = DateTime.Parse("1996-07-13"),
                    Phone = "0983254435",
                    PhoneNumber = "0983254435",
                    Address = "ktx khu B Dĩ An, tỉnh Bình Dương",
                    Role = UserRole.Accountant
                };
                employerNgaTechnical = await CreateApplicationUser(serviceProvider, employerPhuongAccountant, "Abc123456!");
                await EnsureRole(serviceProvider, employerPhuongAccountant.Id, Constants.AccountantRole);
            }
        }

        #endregion

        #region Seeddata_ApplicationUser
        private static async Task<IList<string>> InitApplicationUser(IServiceProvider serviceProvider, string testUserPw)
        {
            IList<string> listApplicationUserID = new List<string>();

            var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@ood.com");
            await EnsureRole(serviceProvider, adminID, Constants.AdminRole);
            listApplicationUserID.Add(adminID);

            // allowed user can create and edit contacts that they create
            var saleID = await EnsureUser(serviceProvider, testUserPw, "sale@ood.com");
            await EnsureRole(serviceProvider, saleID, Constants.SaleRole);
            listApplicationUserID.Add(saleID);

            var warehousemanagerID = await EnsureUser(serviceProvider, testUserPw, "warehousemanager@ood.com");
            await EnsureRole(serviceProvider, warehousemanagerID, Constants.WarehouseManagerRole);
            listApplicationUserID.Add(warehousemanagerID);

            var technicalID = await EnsureUser(serviceProvider, testUserPw, "technical@ood.com");
            await EnsureRole(serviceProvider, technicalID, Constants.TechnicalRole);
            listApplicationUserID.Add(technicalID);

            var adminTwoID = await EnsureUser(serviceProvider, "Abc123456!", "admintwo@gmail.com");
            await EnsureRole(serviceProvider, adminTwoID, Constants.AdminRole);
            listApplicationUserID.Add(adminTwoID);

            // Add Full info Admin user
            var employerToanAdmin = new ApplicationUser
            {
                UserName = "toannguyen@gmail.com",
                Email = "toannguyen@gmail.com",
                FirstName = "Toan",
                LastName = "Nguyen Thanh Thien",
                Birthday = DateTime.Parse("1992-09-20"),
                Phone = "01672699288",
                PhoneNumber = "01672699288",
                Address = "30/15 Đặng Văn Bi, Gò Vấp",
                Role = UserRole.Admin
            };
            employerToanAdmin = await CreateApplicationUser(serviceProvider, employerToanAdmin, "Abc123456!");
            await EnsureRole(serviceProvider, employerToanAdmin.Id, Constants.AdminRole);
            listApplicationUserID.Add(employerToanAdmin.Id);

            // Add Full info Sale user
            var employerThuSale = new ApplicationUser
            {
                UserName = "thunguyen@gmail.com",
                Email = "thunguyen@gmail.com",
                FirstName = "Thư",
                LastName = "Châu Hồng Anh",
                Birthday = DateTime.Parse("1996-03-16"),
                Phone = "0168794582",
                PhoneNumber = "0168794582",
                Address = "80 Cách Mạng Tháng 8, Tân Bình",
                Role = UserRole.Sale
            };
            employerThuSale = await CreateApplicationUser(serviceProvider, employerThuSale, "Abc123456!");
            await EnsureRole(serviceProvider, employerThuSale.Id, Constants.SaleRole);
            listApplicationUserID.Add(employerThuSale.Id);

            // Add Full info WarehouseManager user
            var employerSinhWarehouse = new ApplicationUser
            {
                UserName = "trisinh1996@gmail.com",
                Email = "trisinh1996@gmail.com",
                FirstName = "Sinh",
                LastName = "Nguyễn Tri",
                Birthday = DateTime.Parse("1996-08-10"),
                Phone = "01672699288",
                PhoneNumber = "01672699288",
                Address = "162 Huỳnh Văn Bánh, Quận Phú Nhuận",
                Role = UserRole.WarehouseManager
            };
            employerSinhWarehouse = await CreateApplicationUser(serviceProvider, employerSinhWarehouse, "Abc123456!");
            await EnsureRole(serviceProvider, employerSinhWarehouse.Id, Constants.WarehouseManagerRole);
            listApplicationUserID.Add(employerSinhWarehouse.Id);

            // Add Full info Technical user
            var employerNgaTechnical = new ApplicationUser
            {
                UserName = "ngavtt@gmail.com",
                Email = "ngavtt@gmail.com",
                FirstName = "Nga",
                LastName = "Võ Thị Thúy",
                Birthday = DateTime.Parse("1996-10-05"),
                Phone = "0983254435",
                PhoneNumber = "0983254435",
                Address = "ktx khu B Dĩ An, tỉnh Bình Dương",
                Role = UserRole.Technical
            };
            employerNgaTechnical = await CreateApplicationUser(serviceProvider, employerNgaTechnical, "Abc123456!");
            await EnsureRole(serviceProvider, employerNgaTechnical.Id, Constants.TechnicalRole);
            listApplicationUserID.Add(employerNgaTechnical.Id);

            // Add Full info Accountant user
            var employerPhuongAccountant = new ApplicationUser
            {
                UserName = "phuongnguyen@gmail.com",
                Email = "phuongnguyen@gmail.com",
                FirstName = "Phương",
                LastName = "Nguyễn Lan",
                Birthday = DateTime.Parse("1996-07-13"),
                Phone = "0983254435",
                PhoneNumber = "0983254435",
                Address = "ktx khu B Dĩ An, tỉnh Bình Dương",
                Role = UserRole.Accountant
            };
            employerNgaTechnical = await CreateApplicationUser(serviceProvider, employerPhuongAccountant, "Abc123456!");
            await EnsureRole(serviceProvider, employerPhuongAccountant.Id, Constants.AccountantRole);
            listApplicationUserID.Add(employerPhuongAccountant.Id);

            return listApplicationUserID;
        }
        #endregion

        #region snipset_CreateApplicationUserWithFullInfo
        private static async Task<ApplicationUser> CreateApplicationUser (IServiceProvider serviceProvider, ApplicationUser applicationUser, string password)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByNameAsync(applicationUser.UserName);

            if (user == null)
            {
                user = applicationUser;
                await userManager.CreateAsync(user, password);
            }
            return user;
        }
        #endregion

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