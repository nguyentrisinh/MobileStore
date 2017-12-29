using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MobileStore.Data;
using Microsoft.EntityFrameworkCore;
using MobileStore.Models;

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
            var item = _context.OrderDetail.Include(i => i.Order)
                        .Include(i => i.Item)
                        .ThenInclude(i => i.ModelFromSupplier)
                        .ThenInclude(i => i.StockReceiving)
                        .ThenInclude(i => i.Supplier);

            item = from i in item group i by i.Item.ModelFromSupplierID into newModel select ;

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
                //item = item.Include(i => i.Order)
                //    .Include(i => i.Item)
                //    .ThenInclude(i => i.ModelFromSupplier)
                //    .ThenInclude(i => i.StockReceiving)
                //    .ThenInclude(i => i.Supplier)
                //    .Where(i => i.Item.Name.Contains(searchString));
            }

            // Tinh so luong ban, chenh lech gia, loi nhuan, doanh thu
            //var numberSold = from i in item select i.Item.ModelFromSupplierID;
            //ViewData["NumberSoldParm"] = numberSold.Count();
            ViewData["DiffInPriceParm"] = sortOrder == "diffInPrice" ? "diffInPrice_desc" : "diffInPrice";
            ViewData["RevenueParm"] = sortOrder == "revenue" ? "revenue_desc" : "revenue";

            return View(await item.ToListAsync());
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