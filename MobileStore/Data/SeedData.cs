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
        #region seeddata
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {



            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes we are seeding 2 users both with the same password.
                // The password is set with the following command:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                // Check that database has had anything
                // Look for any movies.
                if (context.ApplicationUser.Any())
                {
                    return;   // DB has been seeded
                }

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

                #region Snipset_Seeddata 

                var constants = new Constant[]
                {
                    new Constant {Name = "Hạn đổi trả", Description ="Thời hạn mà người dùng có thể đổi trả sản phẩm tính từ lúc lập phiếu bảo hành", Parameter = 7}
                };

                foreach (Constant constant in constants)
                {
                    context.Add(constant);
                }

                context.SaveChanges();

                #region Customer
                var customers = new Customer[]
                {
                    new Customer{Name = "Trần Thị Thanh Thảo", Phone = "01672699288", Address = "164 Huỳnh Văn Bánh, quận Phú Nhuận", Birthday = DateTime.Parse("1996-07-13"), Gender = GenderType.Female,
                    Country = "Vietnam"},
                    new Customer{Name = "Trần Đức Duy", Phone = "0983254435", Address = "335/13 Nguyễn Thái Sơn, quận Gò Vấp", Birthday = DateTime.Parse("1992-04-01"), Gender = GenderType.Male,
                    Country = "Vietnam"},
                    new Customer{Name = "Phạm Đình Vy", Phone = "0167254435", Address = "335/13 Nguyễn Thái Sơn, quận Gò Vấp", Birthday = DateTime.Parse("1991-04-01"), Gender = GenderType.Male,
                    Country = "Vietnam"}
                };

                foreach (Customer c in customers)
                {
                    context.Customer.Add(c);
                }

                context.SaveChanges();
                #endregion

                #region Brand
                var brands = new Brand[]
                {
                    new Brand {Name = "Samsung", Country = "Hàn Quốc", Description = "Không có"},
                    new Brand {Name = "Apple", Country = "USA", Description = "" },
                    new Brand {Name = "Oppo", Country = "China", Description = "" },
                    new Brand {Name = "JBL", Country = "USA", Description = "" },
                    new Brand {Name = "ESaver", Country="Vietnam", Description = ""}
                };

                foreach (Brand brand in brands)
                {
                    context.Brand.Add(brand);
                }

                context.SaveChanges();
                #endregion

                #region Supplier
                var suppliers = new Supplier[]
                {
                    new Supplier {Name = "FPT", Address = "Lô 37-39A, đường 19, KCX Tân Thuận, Tân Thuận Đông, Q7", Phone = "+84 7300 2222", Email = "fpt@gmail.com", Code = "500000", Status = SupplierStatus.Active,
                    PicName = "Nguyễn Văn A", PicEmail = "fpt@gmail.com", PicPhone = "+84 7300 2222"},
                    new Supplier {Name = "Viettel", Address = "174 Trần Quang Khải, quận 1", Phone = "0963100900", Email = "viettel@gmail.com", Code = "500000", Status = SupplierStatus.Active,
                    PicName = "Viettel", PicEmail = "viettel@gmail.com", PicPhone = "0963100900"},
                    new Supplier {Name = "The Gioi Di Dong", Address = "128 Trần Quang Khải, P. Tân Định, Q.1", Phone = "18001060", Email = "tgdd@gmail.com", Code = "500000", Status = SupplierStatus.Active,
                    PicName = "The Gioi Di Dong", PicEmail = "tgdd@gmail.com", PicPhone = "18001060"}
                };
                
                foreach (Supplier s in suppliers)
                {
                    context.Supplier.Add(s);
                }

                context.SaveChanges();
                #endregion

                #region Model
                var models = new Model[]
                {
                    new Model {Name = "Samsung Galaxy j7 Pro 64gbs", Color = "Black", Description = "Là chiếc điện thoại cao cấp thời thượng của giới trẻ", Specification = "Chip 8 nhân, Ram 3Gbs, ROM 64Gbs", Type = ModelType.Device,
                    BrandID = brands.Single(i => i.Name == "Samsung").BrandID},
                    new Model {Name = "Samsung Galaxy A8+ (2018)", Color = "Grey", Description = "Là chiếc điện thoại cao cấp thời thượng của giới trẻ", Specification = "Chip 8 nhân, Ram 4Gbs, ROM 64Gbs", Type = ModelType.Device,
                    BrandID = brands.Single(i => i.Name == "Samsung").BrandID},
                    new Model {Name = "IPhone X 64Gbs", Color = "Black", Description = "Smart phone cao cấp", Specification = "Chip 8 nhân, Ram 4Gbs, ROM 64Gbs", Type = ModelType.Device,
                    BrandID = brands.Single(i => i.Name == "Apple").BrandID},
                    new Model {Name = "IPhone 8 128Gbs", Color = "Black", Description = "Smart phone cao cấp", Specification = "Chip 8 nhân, Ram 4Gbs, ROM 64Gbs", Type = ModelType.Device,
                    BrandID = brands.Single(i => i.Name == "Apple").BrandID},
                    new Model {Name = "Tai Nghe JBL T450BT", Color = "Black", Description = "Tai nghe cao cấp", Specification = "", Type = ModelType.Accessory,
                    BrandID = brands.Single(i => i.Name == "JBL").BrandID},
                    new Model {Name = "Dây cáp Micro USB 0.2 m eSaver BST-0728", Color = "White", Description = "Dây cáp sạc", Specification = "", Type = ModelType.Accessory,
                    BrandID = brands.Single(i => i.Name == "ESaver").BrandID}
                };

                foreach (Model m in models)
                {
                    context.Model.Add(m);
                }

                context.SaveChanges();
                #endregion

                #region StockReceiving
                var stockReceivings = new StockReceiving[]
                {
                    new StockReceiving{Date = DateTime.Parse("2017-08-21 7:34:42Z"), SupplierID = suppliers.Single(i => i.Name == "FPT").SupplierID, ApplicationUserID = employerSinhWarehouse.Id },
                    new StockReceiving{Date = DateTime.Parse("2017-10-15 11:34:42Z"), SupplierID = suppliers.Single(i => i.Name == "The Gioi Di Dong").SupplierID, ApplicationUserID = employerSinhWarehouse.Id },
                    new StockReceiving{Date = DateTime.Parse("2017-12-15 11:34:42Z"), SupplierID = suppliers.Single(i => i.Name == "Viettel").SupplierID, ApplicationUserID = employerSinhWarehouse.Id },
                };

                foreach (StockReceiving sr in stockReceivings)
                {
                    context.StockReceiving.Add(sr);
                }

                context.SaveChanges();
                #endregion

                #region ModelFromSupplier
                var modelFromSuppliers = new ModelFromSupplier[]
                {
                    new ModelFromSupplier{Quantity = 2, PriceBought = 22000000, PriceSold = 34000000, Date = DateTime.Parse("2017-08-21 7:34:42Z"), Period = 12, ModelID = models.Single(i => i.Name == "IPhone X 64Gbs").ModelID,
                    StockReceivingID = stockReceivings[0].StockReceivingID},
                    new ModelFromSupplier{Quantity = 1, PriceBought = 15000000, PriceSold = 24000000, Date = DateTime.Parse("2017-08-21 7:34:42Z"), Period = 12, ModelID = models.Single(i => i.Name == "Samsung Galaxy A8+ (2018)").ModelID,
                    StockReceivingID = stockReceivings[0].StockReceivingID},
                    new ModelFromSupplier{Quantity = 3, PriceBought = 800000, PriceSold = 1200000, Date = DateTime.Parse("2017-08-21 7:34:42Z"), Period = 12, ModelID = models.Single(i => i.Name == "Tai Nghe JBL T450BT").ModelID,
                    StockReceivingID = stockReceivings[0].StockReceivingID},
                    new ModelFromSupplier{Quantity = 2, PriceBought = 800000, PriceSold = 1200000, Date = DateTime.Parse("2017-10-15 11:34:42Z"), Period = 12, ModelID = models.Single(i => i.Name == "Tai Nghe JBL T450BT").ModelID,
                    StockReceivingID = stockReceivings[1].StockReceivingID},
                    new ModelFromSupplier{Quantity = 2, PriceBought = 80000, PriceSold = 150000, Date = DateTime.Parse("2017-10-15 11:34:42Z"), Period = 3, ModelID = models.Single(i => i.Name == "Dây cáp Micro USB 0.2 m eSaver BST-0728").ModelID,
                    StockReceivingID = stockReceivings[1].StockReceivingID},
                    new ModelFromSupplier{Quantity = 2, PriceBought = 22000000, PriceSold = 32000000, Date = DateTime.Parse("2017-12-21 7:34:42Z"), Period = 12, ModelID = models.Single(i => i.Name == "IPhone X 64Gbs").ModelID,
                    StockReceivingID = stockReceivings[0].StockReceivingID},
                };

                foreach (ModelFromSupplier mfs in modelFromSuppliers)
                {
                    context.ModelFromSupplier.Add(mfs);
                }

                context.SaveChanges();
                #endregion

                #region Item
                var items = new Item[]
                {
                    new Item{IMEI = "2546CQC189CQ438CSA", SerializerNumber = "220167955897642", Note = "", Status = ItemStatus.Sold, ModelFromSupplierID = modelFromSuppliers[0].ModelFromSupplierID,
                    ModelID = models.Single(i => i.Name == "IPhone X 64Gbs").ModelID},
                    new Item{IMEI = "QC587QC189C8468CSA", SerializerNumber = "387457952854642", Note = "", Status = ItemStatus.InStock, ModelFromSupplierID = modelFromSuppliers[0].ModelFromSupplierID,
                    ModelID = models.Single(i => i.Name == "IPhone X 64Gbs").ModelID},
                    new Item{IMEI = "59189VEB69C8468CSA", SerializerNumber = "387457952854642", Note = "", Status = ItemStatus.Sold, ModelFromSupplierID = modelFromSuppliers[1].ModelFromSupplierID,
                    ModelID = models.Single(i => i.Name == "Samsung Galaxy A8+ (2018)").ModelID},
                    // Model From Supplier [2] - 3 
                    new Item{IMEI = "8642134FW528468CSA", SerializerNumber = "487995313284612", Note = "", Status = ItemStatus.Sold, ModelFromSupplierID = modelFromSuppliers[2].ModelFromSupplierID,
                    ModelID = models.Single(i => i.Name == "Tai Nghe JBL T450BT").ModelID},
                    new Item{IMEI = "CE468CQF2FW87VWSCE", SerializerNumber = "652217985431287", Note = "", Status = ItemStatus.Sold, ModelFromSupplierID = modelFromSuppliers[2].ModelFromSupplierID,
                    ModelID = models.Single(i => i.Name == "Tai Nghe JBL T450BT").ModelID},
                    new Item{IMEI = "PIV477VWV658VWC249", SerializerNumber = "320549785423187", Note = "", Status = ItemStatus.InStock, ModelFromSupplierID = modelFromSuppliers[2].ModelFromSupplierID,
                    ModelID = models.Single(i => i.Name == "Tai Nghe JBL T450BT").ModelID},
                    // Model From Supplier [3] - 2
                    new Item{IMEI = "5846AVRW69C8468CSA", SerializerNumber = "587643215854642", Note = "", Status = ItemStatus.InStock, ModelFromSupplierID = modelFromSuppliers[3].ModelFromSupplierID,
                    ModelID = models.Single(i => i.Name == "Tai Nghe JBL T450BT").ModelID},
                    new Item{IMEI = "589CQVLC5495V68CSA", SerializerNumber = "302164897524642", Note = "", Status = ItemStatus.InStock, ModelFromSupplierID = modelFromSuppliers[3].ModelFromSupplierID,
                    ModelID = models.Single(i => i.Name == "Tai Nghe JBL T450BT").ModelID},
                    // Model From Supplier [4] - 2
                    new Item{IMEI = "TY7985CQC46CQE879V", SerializerNumber = "978521347852794", Note = "", Status = ItemStatus.Sold, ModelFromSupplierID = modelFromSuppliers[4].ModelFromSupplierID,
                    ModelID = models.Single(i => i.Name == "Dây cáp Micro USB 0.2 m eSaver BST-0728").ModelID},
                    new Item{IMEI = "CV7854VWV6216VWV58", SerializerNumber = "217985462318746", Note = "", Status = ItemStatus.InStock, ModelFromSupplierID = modelFromSuppliers[4].ModelFromSupplierID,
                    ModelID = models.Single(i => i.Name == "Dây cáp Micro USB 0.2 m eSaver BST-0728").ModelID}
                };

                foreach (Item i in items)
                {
                    context.Item.Add(i);
                }

                context.SaveChanges();
                #endregion

                #region Order 
                var orders = new Order[]
                {
                    new Order{Total = 34000000, Date = DateTime.Parse("2017-08-29 7:34:42Z"), IsPrinted = true, CustomerID = customers[0].CustomerID, ApplicationUserID = employerThuSale.Id},
                    new Order{Total = 25200000, Date = DateTime.Parse("2017-09-05 11:34:42Z"), IsPrinted = true, CustomerID = customers[1].CustomerID, ApplicationUserID = saleID},
                    new Order{Total = 1350000, Date = DateTime.Parse("2017-09-16 15:24:42Z"), IsPrinted = false, CustomerID = customers[2].CustomerID, ApplicationUserID = employerThuSale.Id},
                };
                
                foreach (Order order in orders)
                {
                    context.Order.Add(order);
                }

                context.SaveChanges();
                #endregion

                #region OrderDetail 
                var orderDetails = new OrderDetail[]
                {
                    new OrderDetail{PriceSold = 34000000, ItemID = items[0].ItemID, OrderID = orders[0].OrderID},
                    new OrderDetail{PriceSold = 24000000, ItemID = items[2].ItemID, OrderID = orders[1].OrderID},
                    new OrderDetail{PriceSold = 1200000, ItemID = items[3].ItemID, OrderID = orders[1].OrderID},
                    new OrderDetail{PriceSold = 1200000, ItemID = items[4].ItemID, OrderID = orders[2].OrderID},
                    new OrderDetail{PriceSold = 150000, ItemID = items[8].ItemID, OrderID = orders[2].OrderID},
                };

                foreach (OrderDetail orderDetail in orderDetails)
                {
                    context.OrderDetail.Add(orderDetail);
                }

                context.SaveChanges();
                #endregion

                #region WarrantyCard 
                var warrantyCards = new WarrantyCard[]
                {
                    new WarrantyCard{NumberOfWarranty = 0, StartDate = DateTime.Parse("2017-08-29"), EndDate = DateTime.Parse("2018-08-29"), ItemID = items[0].ItemID, IsPrinted = true, IsDisabled = false,
                    TransactionCode = Guid.NewGuid(), ApplicationUserID = employerThuSale.Id},
                    new WarrantyCard{NumberOfWarranty = 0, StartDate = DateTime.Parse("2017-09-05"), EndDate = DateTime.Parse("2018-09-05"), ItemID = items[2].ItemID, IsPrinted = true, IsDisabled = false,
                    TransactionCode = Guid.NewGuid(), ApplicationUserID = employerThuSale.Id },
                    new WarrantyCard{NumberOfWarranty = 0, StartDate = DateTime.Parse("2017-09-05"), EndDate = DateTime.Parse("2018-09-05"), ItemID = items[3].ItemID, IsPrinted = true, IsDisabled = false,
                    TransactionCode = Guid.NewGuid(), ApplicationUserID = employerThuSale.Id},
                    new WarrantyCard{NumberOfWarranty = 0, StartDate = DateTime.Parse("2017-09-16"), EndDate = DateTime.Parse("2018-09-16"), ItemID = items[4].ItemID, IsPrinted = false, IsDisabled = false,
                    TransactionCode = Guid.NewGuid(), ApplicationUserID = employerThuSale.Id},
                    new WarrantyCard{NumberOfWarranty = 0, StartDate = DateTime.Parse("2017-09-16"), EndDate = DateTime.Parse("2017-12-16"), ItemID = items[8].ItemID, IsPrinted = false, IsDisabled = false,
                    TransactionCode = Guid.NewGuid(), ApplicationUserID = employerThuSale.Id},
                };

                foreach (WarrantyCard warrantyCard in warrantyCards)
                {
                    context.WarrantyCard.Add(warrantyCard);
                }

                context.SaveChanges();
                #endregion

                #endregion
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