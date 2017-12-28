using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MobileStore.Data;
using MobileStore.Models;
using MobileStore.Models.StockReceivingViewModels;

namespace MobileStore.Controllers
{
    public class SellableItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SellableItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["NameParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "name";
            //ViewData["ModelParm"] = sortOrder == "model" ? "model_desc" : "model";
            ViewData["CurrentSort"] = sortOrder;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
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
                    item = item.OrderByDescending(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;

                case "name_desc":
                    item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;
                default:
                    item = item.OrderBy(i => i.Name).Include(i => i.Model).Include(i => i.ModelFromSupplier);
                    break;
            }
            int pageSize = 1;

            return View(await PaginatedList<Item>.CreateAsync(item.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: Items/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var item = await _context.Item
        //        .Include(i => i.Model)
        //        .Include(i => i.ModelFromSupplier)
        //        .SingleOrDefaultAsync(m => m.ItemID == id);
        //    if (item == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(item);
        //}
    }
}
