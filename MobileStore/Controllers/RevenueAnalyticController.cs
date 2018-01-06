using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileStore.Data;
using MobileStore.Models;
using MobileStore.Models.ApplicationUserViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MobileStore.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.EntityFrameworkCore;
using MobileStore.Models.RevenueAnalyticViewModels;

namespace MobileStore.Controllers
{
    public class RevenueAnalyticController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RevenueAnalyticController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Index()
        {
            //RevenueAnalyticViewModel revenueAnalyticViewModel = new RevenueAnalyticViewModel();
            List<RevenueListViewModel> revenueListModel = new List<RevenueListViewModel>();
            List<string> listDate = new List<string>();
            List<double> totalSolds = new List<double>();
            List<double> totalRevenues = new List<double>();
            //var orders = from ent in _context.Order
            //             select ent;

            //var orders = _context.Set<Order>().Include("OrderDetails")
            //    .Include("Item").Include("ModelFromSupplier");

            var orders = _context.Order;
                //.ThenInclude(ef => ef.Item)
                //.ThenInclude(ef => ef.ModelFromSupplier);

            var orderGrouped = from p in orders
                               group p by new { month = p.Date.Month, year = p.Date.Year } into d
                               select new {
                                   month = d.Key.month,
                                   year = d.Key.year,
                                   dt = string.Format("{0}/{1}", d.Key.month, d.Key.year),
                                   count = d.Count(),
                                   //total = d.Sum(order => order.Total).ToString()
                               };

            //orderGrouped.ForEachAsync(group =>
            //{
            //    var orderList = _context.Order.Where(order => order.Date.Month == group.month && order.Date.Year == group.year);

            //    var TotalSold = orderList.Sum(order => order.Total);

            //    RevenueListViewModel revenueListViewModel = new RevenueListViewModel
            //    {
            //        Month = group.month,
            //        Year = group.year,
            //        Total = TotalSold
            //    };

            //    revenueAnalyticViewModel.RevenueListViewModels.Add(revenueListViewModel);
            //});

            foreach(var group in orderGrouped)
            {
                var orderList = _context.Order.Where(order => order.Date.Month == group.month && order.Date.Year == group.year);

                var TotalSold = orderList.Sum(order => order.Total);

                // Counting Revenue 
                var orderListRevenue = _context.Order.Where(order => order.Date.Month == group.month && order.Date.Year == group.year).Include(order => order.OrderDetails)
                    .ThenInclude(orderDetail => orderDetail.Item)
                    .ThenInclude(item => item.ModelFromSupplier);

                double TotalPrice = 0;

                foreach(var order in orderListRevenue)
                {
                    TotalPrice += order.OrderDetails.Sum(orderDetail => orderDetail.Item.ModelFromSupplier.PriceBought);
                }

                RevenueListViewModel revenue = new RevenueListViewModel()
                {
                    Month = group.month,
                    Year = group.year,
                    Total = TotalSold,
                    Revenue = TotalSold - TotalPrice
                };

                revenueListModel.Add(revenue);

                listDate.Add(group.dt);
                totalSolds.Add(TotalSold);
                totalRevenues.Add(TotalSold - TotalPrice);
            }

            ViewData["RevenueList"] = revenueListModel;
            ViewData["ListDate"] = listDate;
            ViewData["TotalSold"] = totalSolds;
            ViewData["TotalRevenue"] = totalRevenues;

            return View();
        }
    }
}