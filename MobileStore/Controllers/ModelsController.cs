﻿using System;
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
    public class ModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Models
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

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
            var applicationDbContext = from m in _context.Model
                                       .Include("Brand")
                                       select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                applicationDbContext =applicationDbContext.Where(m => m.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    applicationDbContext = applicationDbContext.OrderByDescending(s => s.Name);
                    break;
                default:
                    applicationDbContext = applicationDbContext.OrderBy(s => s.Name);
                    break;

            }


            int pageSize = 12;

            return View(await PaginatedList<Model>.CreateAsync(applicationDbContext.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: Models/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _context.Model
                .Include(m => m.Brand)
                .SingleOrDefaultAsync(m => m.ModelID == id);
            if (model == null)
            {
                return NotFound();
            }

            ViewData["TotalItems"] = await _context.Item.Where(m => m.ModelID == id).CountAsync();

            ViewData["InstockItems"] =await _context.Item.Where(m => m.ModelID == id && m.Status==ItemStatus.InStock).CountAsync();

            ViewData["SoldItems"] = await _context.Item.Where(m => m.ModelID == id && m.Status == ItemStatus.Sold).CountAsync();

            ViewData["ReturnedItems"] = await _context.Item.Where(m => m.ModelID == id && m.Status == ItemStatus.Returned).CountAsync();

            return View(model);
        }

        // GET: Models/Create
        [Authorize(Roles = "WarehouseManager, Admin")]
        public IActionResult Create()
        {
            ViewData["BrandID"] = new SelectList(_context.Brand, "BrandID", "Name");
            return View();
        }

        // POST: Models/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Create([Bind("ModelID,Name,Color,Description,Specification,Type,BrandID")] Model model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandID"] = new SelectList(_context.Brand, "BrandID", "Name", model.BrandID);
            return View(model);
        }

        // GET: Models/Edit/5
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _context.Model.SingleOrDefaultAsync(m => m.ModelID == id);
            if (model == null)
            {
                return NotFound();
            }
            ViewData["BrandID"] = new SelectList(_context.Brand, "BrandID", "Name", model.BrandID);
            return View(model);
        }

        // POST: Models/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ModelID,Name,Color,Description,Specification,Type,BrandID")] Model model)
        {
            if (id != model.ModelID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelExists(model.ModelID))
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
            ViewData["BrandID"] = new SelectList(_context.Brand, "BrandID", "Name", model.BrandID);
            return View(model);
        }

        // GET: Models/Delete/5
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _context.Model
                .Include(m => m.Brand)
                .SingleOrDefaultAsync(m => m.ModelID == id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: Models/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "WarehouseManager, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var model = await _context.Model.SingleOrDefaultAsync(m => m.ModelID == id);
            _context.Model.Remove(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModelExists(int id)
        {
            return _context.Model.Any(e => e.ModelID == id);
        }
    }
}
