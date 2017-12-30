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

namespace MobileStore.Controllers
{
    public class SoldItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SoldItemsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Items
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var orderDetail = _context.OrderDetail.Include(i => i.Order)
                            .Include(i => i.Item)
                            .ThenInclude(i => i.ModelFromSupplier)
                            .ThenInclude(i => i.StockReceiving)
                            .ThenInclude(i => i.Supplier);

            // Nhóm theo nhà cung cấp và theo giá bán thực,
            // vì item có thể có nhiều nhà cung cấp với giá mua khác nhau
            // và giá bán thực cũng có thể khác nhau (đợt khuyến mãi)
            var soldItemGroup = orderDetail.GroupBy(o => new {o.Item.ModelFromSupplierID, o.PriceSold});
            List<SoldItems> returnedSoldItems = new List<SoldItems>();
            soldItemGroup.ForEach(k =>
            {
                var first = k.FirstOrDefault();
                var numberSold = k.Count();
                var priceBought = first.Item.ModelFromSupplier.PriceBought;
                var priceSold = first.Item.ModelFromSupplier.PriceSold;
                var actualPriceSold = first.PriceSold;

                returnedSoldItems.Add(new SoldItems()
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
                });
            });

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "name";
            ViewData["NumberSoldSortParm"] = sortOrder == "numberSold" ? "numberSold_desc" : "numberSold";
            ViewData["PriceBoughtSortParm"] = sortOrder == "priceBought" ? "priceBought_desc" : "priceBought";
            ViewData["PriceSoldSortParm"] = sortOrder == "priceSold" ? "priceSold_desc" : "priceSold";
            ViewData["DiffInPriceSortParm"] = sortOrder == "diffInPrice" ? "diffInPrice_desc" : "diffInPrice";
            ViewData["RevenueSortParm"] = sortOrder == "revenue" ? "revenue_desc" : "revenue";
            ViewData["DateSoldSortParm"] = sortOrder == "dateSold" ? "dateSold_desc" : "dateSold";

            // Filtering
            if (searchString != null)
            {
                page = 1;
            } else {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                soldItemGroup.Select(i => i).Where(i => i.FirstOrDefault().Item.Name == searchString);
            }

            return View(returnedSoldItems);
            //switch (sortOrder)
            //{
            //    case "name":
            //        //item = item.OrderByDescending(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
            //        break;

            //    case "name_desc":
            //        item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
            //        break;

            //    case "numberSold":

            //        break;

            //    case "numberSold_desc":
            //        //item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
            //        break;

            //    case "priceBought":
            //        item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
            //        break;

            //    case "priceBought_desc":
            //        item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
            //        break;

            //    case "priceSold":
            //        item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
            //        break;

            //    case "priceSold_desc":
            //        item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
            //        break;

            //    case "diffInPrice":
            //        item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
            //        break;

            //    case "diffInPrice_desc":
            //        item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
            //        break;

            //    case "revenue":
            //        item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
            //        break;

            //    case "revenue_desc":
            //        item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
            //        break;

            //    default:
            //        item = _context.Item.Include(i => i.Model).Include(i => i.ModelFromSupplier);
            //        //_context.Item.Include(i => i.Model).Include(i => i.ModelFromSupplier);
            //        break;
            //}

            //int pageSize = 1;
            //return View(await PaginatedList<Item>.CreateAsync(item.AsNoTracking(), page ?? 1, pageSize));
            //            return View(await item.ToListAsync());
        }
    }

}
