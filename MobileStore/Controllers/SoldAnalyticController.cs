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

            var months = soldItems.Select(i => i.DateSold.Month).Distinct();
            var quater = months.Select(i => QuarterCalculate(i)).Distinct();
            var year = soldItems.Select(i => i.DateSold.Year).Distinct();
            ViewData["MonthsQuatersYears"] = new SelectList(months);

            // Total calculate
            ViewData["TotalNumberSold"] = soldItems.Sum(i => i.NumberSold);
            ViewData["TotalPriceBought"] = soldItems.Sum(i => i.PriceBought * i.NumberSold);
            ViewData["TotalPriceSold"] = soldItems.Sum(i => i.PriceSold);
            ViewData["TotalActualPriceSold"] = soldItems.Sum(i => i.ActualPriceSold * i.NumberSold);
            ViewData["TotalDiffInPrice"] = soldItems.Sum(i => i.DiffInPrice * i.NumberSold);
            ViewData["TotalRevenue"] = soldItems.Sum(i => i.Revenue * i.NumberSold);

            return View(soldItems);
        }

        private IEnumerable<SoldAnalytic> GetSoldItems()
        {
            var orderDetail = _context.OrderDetail.Include(i => i.Order)
                            .Include(i => i.Item)
                            .ThenInclude(i => i.ModelFromSupplier)
                            .ThenInclude(i => i.StockReceiving)
                            .ThenInclude(i => i.Supplier);

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
                    Name = first.Item.Name,
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

        private string QuarterCalculate(int month)
        {
            string quarter = "Quý 1";
            if (month >= 4 && month <= 6)
                quarter = "Quý 2";
            else if (month >= 7 && month <= 9)
                quarter = "Quý 3";
            else if (month >= 10)
                quarter = "Quý 4";

            return quarter;
        }

        [HttpPost]
        public ActionResult Index(string TimeUnit, int MQYValue)
        {
            var soldItems = GetSoldItems();
            //switch (TimeUnit)
            //{
            //    case "Year":
            //        var year = soldItems.Select(i => i.DateSold.Year).Distinct();
            //        ViewData["MonthsQuatersYears"] = new SelectList(year);
            //        break;

            //    case "Quarter":
            //        var quater = soldItems.Select(i => QuarterCalculate(i.DateSold.Month)).Distinct();
            //        ViewData["MonthsQuatersYears"] = new SelectList(quater);
            //        break;

            //    case "Month":
            //    default:
            //        var months = soldItems.Select(i => i.DateSold.Month).Distinct();
            //        ViewData["MonthsQuatersYears"] = new SelectList(months);
            //        break;
            //}
            var year = soldItems.Select(i => i.DateSold.Year).Distinct();
            ViewData["MonthsQuatersYears"] = new SelectList(year);

            var returnedSoldItems = soldItems.Select(i => i).Where(i => i.DateSold.Month == MQYValue);
            return View(returnedSoldItems);
        }
}
}
