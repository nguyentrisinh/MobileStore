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
            ViewData["NameParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "name";
            ViewData["NumberSoldParm"] = sortOrder == "numberSold" ? "numberSold_desc" : "numberSold";
            ViewData["PriceBoughtParm"] = sortOrder == "priceBought" ? "priceBought_desc" : "priceBought";
            ViewData["PriceSoldParm"] = sortOrder == "priceSold" ? "priceSold_desc" : "priceSold";
            ViewData["DiffInPriceParm"] = sortOrder == "diffInPrice" ? "diffInPrice_desc" : "diffInPrice";
            ViewData["RevenueParm"] = sortOrder == "revenue" ? "revenue_desc" : "revenue";

            if (searchString != null)
            {
                page = 1;
            } else {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            var item = from m in _context.Item select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                item = item.Include(i => i.Model).Include(i => i.ModelFromSupplier).Where(i => i.Status == ItemStatus.InStock && i.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name":
                    //item = item.OrderByDescending(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;

                case "name_desc":
                    item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;

                case "numberSold":

                    break;

                case "numberSold_desc":
                    //item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;

                case "priceBought":
                    item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;

                case "priceBought_desc":
                    item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;

                case "priceSold":
                    item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;

                case "priceSold_desc":
                    item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;

                case "diffInPrice":
                    item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;

                case "diffInPrice_desc":
                    item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;

                case "revenue":
                    item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;

                case "revenue_desc":
                    item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;

                default:
                    item = _context.Item.Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    //_context.Item.Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;
            }

            //int pageSize = 1;
            //return View(await PaginatedList<Item>.CreateAsync(item.AsNoTracking(), page ?? 1, pageSize));
            return View(await item.ToListAsync());
        }
    }
}