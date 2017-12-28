using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MobileStore.Data;
using MobileStore.Models;

namespace MobileStore.Controllers
{
    public class BrandsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrandsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Brands
        public async Task<IActionResult> Index(string SearchString, string sortOrder,string currentFilter, int?page)
        {
           
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CountrySortParm"] = String.IsNullOrEmpty(sortOrder) ? "country_desc" : "country_asc";

            
            ViewData["CurrentSort"] = sortOrder;

            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }
            ViewData["CurrentFilter"] = SearchString;
            var brand = from b in _context.Brand select b;

            if (!String.IsNullOrEmpty(SearchString))
            {
                brand = brand.Where(s => s.Name.Contains(SearchString) || s.Country.Contains(SearchString));
            }


            switch (sortOrder)
            {
                case "name_desc":
                    brand = brand.OrderByDescending(s => s.Name);
                    break;
                case "country_desc":
                    brand = brand.OrderByDescending(s => s.Country);
                    break;
                case "country_asc":
                    brand = brand.OrderBy(s => s.Country);
                    break;
                default:
                    brand = brand.OrderBy(s => s.Name);
                    break;

            }
            //return View(await brand.ToListAsync());
            int pageSize = 2;
            return View(await PaginatedList<Brand>.CreateAsync(brand.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brand
                .SingleOrDefaultAsync(m => m.BrandID == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Brands/Create
        [Authorize(Roles = "WarehouseManager, Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Create([Bind("BrandID,Name,Country,Description")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: Brands/Edit/5
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brand.SingleOrDefaultAsync(m => m.BrandID == id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Brands/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("BrandID,Name,Country,Description")] Brand brand)
        {
            if (id != brand.BrandID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.BrandID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: Brands/Delete/5
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brand
                .SingleOrDefaultAsync(m => m.BrandID == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "WarehouseManager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await _context.Brand.SingleOrDefaultAsync(m => m.BrandID == id);
            _context.Brand.Remove(brand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
            return _context.Brand.Any(e => e.BrandID == id);
        }
    }
}
