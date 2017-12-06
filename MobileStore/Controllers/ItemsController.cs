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
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Item.Include(i => i.Model).Include(i => i.ModelFromSupplier);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(i => i.Model)
                .Include(i => i.ModelFromSupplier)
                .SingleOrDefaultAsync(m => m.ItemID == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "ModelID");
            ViewData["ModelFromSupplierID"] = new SelectList(_context.ModelFromSupplier, "ModelFromSupplierID", "ModelFromSupplierID");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockReceivingViewModel stockReceivingVM)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stockReceivingVM.Item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "ModelID", stockReceivingVM.Item.ModelID);
            ViewData["ModelFromSupplierID"] = new SelectList(_context.ModelFromSupplier, "ModelFromSupplierID", "ModelFromSupplierID", stockReceivingVM.Item.ModelFromSupplierID);
            return RedirectToAction("Create","ModelFromSuppliers");
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item.SingleOrDefaultAsync(m => m.ItemID == id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "ModelID", item.ModelID);
            ViewData["ModelFromSupplierID"] = new SelectList(_context.ModelFromSupplier, "ModelFromSupplierID", "ModelFromSupplierID", item.ModelFromSupplierID);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemID,Name,IMEI,SerializerNumber,Note,Status,ModelFromSupplierID,ModelID")] Item item)
        {
            if (id != item.ItemID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.ItemID))
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
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "ModelID", item.ModelID);
            ViewData["ModelFromSupplierID"] = new SelectList(_context.ModelFromSupplier, "ModelFromSupplierID", "ModelFromSupplierID", item.ModelFromSupplierID);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(i => i.Model)
                .Include(i => i.ModelFromSupplier)
                .SingleOrDefaultAsync(m => m.ItemID == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Item.SingleOrDefaultAsync(m => m.ItemID == id);
            _context.Item.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.ItemID == id);
        }
    }
}
