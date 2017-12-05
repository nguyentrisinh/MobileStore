using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MobileStore.Data;
using MobileStore.Models;

namespace MobileStore.Controllers
{
    public class WarrantyCardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WarrantyCardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WarrantyCards
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WarrantyCard.Include(w => w.Item);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: WarrantyCards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warrantyCard = await _context.WarrantyCard
                .Include(w => w.Item)
                .SingleOrDefaultAsync(m => m.WarrantyCardID == id);
            if (warrantyCard == null)
            {
                return NotFound();
            }

            return View(warrantyCard);
        }

        // GET: WarrantyCards/Create
        public IActionResult Create()
        {
            ViewData["ItemID"] = new SelectList(_context.Item, "ItemID", "Name");
            return View();
        }

        // POST: WarrantyCards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WarrantyCardID,NumberOfWarranty,StartDate,EndDate,Period,ItemID")] WarrantyCard warrantyCard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(warrantyCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemID"] = new SelectList(_context.Item, "ItemID", "Name", warrantyCard.ItemID);
            return View(warrantyCard);
        }

        // GET: WarrantyCards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warrantyCard = await _context.WarrantyCard.SingleOrDefaultAsync(m => m.WarrantyCardID == id);
            if (warrantyCard == null)
            {
                return NotFound();
            }
            ViewData["ItemID"] = new SelectList(_context.Item, "ItemID", "Name", warrantyCard.ItemID);
            return View(warrantyCard);
        }

        // POST: WarrantyCards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WarrantyCardID,NumberOfWarranty,StartDate,EndDate,Period,ItemID")] WarrantyCard warrantyCard)
        {
            if (id != warrantyCard.WarrantyCardID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(warrantyCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarrantyCardExists(warrantyCard.WarrantyCardID))
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
            ViewData["ItemID"] = new SelectList(_context.Item, "ItemID", "Name", warrantyCard.ItemID);
            return View(warrantyCard);
        }

        // GET: WarrantyCards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warrantyCard = await _context.WarrantyCard
                .Include(w => w.Item)
                .SingleOrDefaultAsync(m => m.WarrantyCardID == id);
            if (warrantyCard == null)
            {
                return NotFound();
            }

            return View(warrantyCard);
        }

        // POST: WarrantyCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var warrantyCard = await _context.WarrantyCard.SingleOrDefaultAsync(m => m.WarrantyCardID == id);
            _context.WarrantyCard.Remove(warrantyCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WarrantyCardExists(int id)
        {
            return _context.WarrantyCard.Any(e => e.WarrantyCardID == id);
        }
    }
}
