using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileStore.Data;
using Microsoft.EntityFrameworkCore;
using MobileStore.Models;
using System.Collections.Generic;
using System.Collections;
using MoreLinq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MobileStore.Controllers
{
    public class SoldAnalyticController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SoldAnalyticController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Items
        public async Task<IActionResult> Index()
        {
            var soldItems = GetSoldItems();
            var months = soldItems.Select(i =>
                new
                {
                    month = i.DateSold.Month,
                    year = i.DateSold.Year
                }).Distinct();
            ViewData["Months"] = new SelectList(months.Select(i => i.month.ToString() + "/" + i.year.ToString()));
            ViewData["MonthText"] = "...";

            return View(new List<SoldAnalytic>());
        }

        private IEnumerable<SoldAnalytic> GetSoldItems()
        {
            var orderDetail = _context.OrderDetail.Include(i => i.Order)
                            .Include(i => i.Item)
                            .ThenInclude(i => i.ModelFromSupplier)
                            .ThenInclude(i => i.StockReceiving)
                            .ThenInclude(i => i.Supplier)
                            .Include(i => i.Item).ThenInclude(i => i.Model);

            // Nhóm theo nhà cung cấp và theo giá bán thực,
            // vì item có thể có nhiều nhà cung cấp với giá mua khác nhau
            // và giá bán thực cũng có thể khác nhau (đợt khuyến mãi)
            List<SoldAnalytic> soldItems = new List<SoldAnalytic>();
            var soldItemGroup = orderDetail.GroupBy(o => new { o.Item.ModelFromSupplierID, o.PriceSold });
            soldItemGroup.ForEach(k =>
            {
                var first = k.FirstOrDefault();
                var numberSold = k.Count();
                var priceBought = first.Item.ModelFromSupplier.PriceBought;
                var priceSold = first.Item.ModelFromSupplier.PriceSold;
                var actualPriceSold = first.PriceSold;

                soldItems.Add(new SoldAnalytic()
                {
                    ModelFromSupplierID = first.Item.ModelFromSupplierID,
                    Name = first.Item.Model.Name,
                    NumberSold = numberSold,
                    PriceBought = priceBought,
                    PriceSold = priceSold,
                    ActualPriceSold = actualPriceSold,
                    DiffInPrice = priceSold - actualPriceSold, // chênh lệch giữa giá bán thực (có khuyến mãi) và đơn giá bán
                    Revenue = actualPriceSold - priceBought, // lợi nhuận sinh ra từ chênh lệch giữa giá bán thực và giá mua trên 1 sản phẩm
                    TotalRevenue = (actualPriceSold - priceBought) * numberSold, // tổng lợi nhuận trên n sản phẩm
                    SupplierName = first.Item.ModelFromSupplier.StockReceiving.Supplier.Name,
                    DateSold = first.Order.Date,
                });
            });
            return soldItems;
        }

        private int QuarterCalculate(int month)
        {
            int quarter = 1;
            if (month >= 4 && month <= 6)
                quarter = 2;
            else if (month >= 7 && month <= 9)
                quarter = 3;
            else if (month >= 10)
                quarter = 4;

            return quarter;
        }

        [HttpPost]
        public ActionResult Index(string Months)
        {
            var soldItems = GetSoldItems();
            ViewData["MonthText"] = Months;

            var months = soldItems.Select(i =>
            new
            {
                month = i.DateSold.Month,
                year = i.DateSold.Year
            }).Distinct();
            ViewData["Months"] = new SelectList(months.Select(i => i.month.ToString() + "/" + i.year.ToString()));

            var monthSoldObj = Months.Split("/");
            var returnedSoldItems = soldItems.Select(i => i)
                .Where(i =>
                   i.DateSold.Month == Int32.Parse(monthSoldObj[0])
                   && i.DateSold.Year == Int32.Parse(monthSoldObj[1])
                );
            // Total calculate
            ViewData["TotalNumberSold"] = returnedSoldItems.Sum(i => i.NumberSold);
            ViewData["TotalPriceBought"] = returnedSoldItems.Sum(i => i.PriceBought * i.NumberSold);
            ViewData["TotalPriceSold"] = returnedSoldItems.Sum(i => i.PriceSold);
            ViewData["TotalActualPriceSold"] = returnedSoldItems.Sum(i => i.ActualPriceSold * i.NumberSold);
            ViewData["TotalDiffInPrice"] = returnedSoldItems.Sum(i => i.DiffInPrice * i.NumberSold);
            ViewData["TotalRevenue"] = returnedSoldItems.Sum(i => i.Revenue * i.NumberSold);

            return View(returnedSoldItems);
        }
}
}
