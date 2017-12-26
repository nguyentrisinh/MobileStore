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
    public class ReturnItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReturnItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ReturnItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ReturnItem.Include(r => r.ApplicationUser).Include(r => r.NewItem).Include(r => r.OldItem);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ReturnItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var returnItem = await _context.ReturnItem
                .Include(r => r.ApplicationUser)
                .Include(r => r.NewItem)
                .Include(r => r.OldItem)
                .SingleOrDefaultAsync(m => m.ReturnItemID == id);
            if (returnItem == null)
            {
                return NotFound();
            }

            return View(returnItem);
        }

        // GET: ReturnItems/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUser, "Id", "Id");
            ViewData["NewItemID"] = new SelectList(_context.Item, "ItemID", "ItemID");
            ViewData["OldItemID"] = new SelectList(_context.Item, "ItemID", "ItemID");
            return View();
        }

        // POST: ReturnItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReturnItemID,ReturnDate,DefectInfo,NewItemID,OldItemID,ApplicationUserID")] ReturnItem returnItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(returnItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUser, "Id", "Id", returnItem.ApplicationUserID);
            ViewData["NewItemID"] = new SelectList(_context.Item, "ItemID", "ItemID", returnItem.NewItemID);
            ViewData["OldItemID"] = new SelectList(_context.Item, "ItemID", "ItemID", returnItem.OldItemID);
            return View(returnItem);
        }

        // GET: ReturnItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var returnItem = await _context.ReturnItem.SingleOrDefaultAsync(m => m.ReturnItemID == id);
            if (returnItem == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUser, "Id", "Id", returnItem.ApplicationUserID);
            ViewData["NewItemID"] = new SelectList(_context.Item, "ItemID", "ItemID", returnItem.NewItemID);
            ViewData["OldItemID"] = new SelectList(_context.Item, "ItemID", "ItemID", returnItem.OldItemID);
            return View(returnItem);
        }

        // POST: ReturnItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReturnItemID,ReturnDate,DefectInfo,NewItemID,OldItemID,ApplicationUserID")] ReturnItem returnItem)
        {
            if (id != returnItem.ReturnItemID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(returnItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReturnItemExists(returnItem.ReturnItemID))
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
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUser, "Id", "Id", returnItem.ApplicationUserID);
            ViewData["NewItemID"] = new SelectList(_context.Item, "ItemID", "ItemID", returnItem.NewItemID);
            ViewData["OldItemID"] = new SelectList(_context.Item, "ItemID", "ItemID", returnItem.OldItemID);
            return View(returnItem);
        }

        // GET: ReturnItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var returnItem = await _context.ReturnItem
                .Include(r => r.ApplicationUser)
                .Include(r => r.NewItem)
                .Include(r => r.OldItem)
                .SingleOrDefaultAsync(m => m.ReturnItemID == id);
            if (returnItem == null)
            {
                return NotFound();
            }

            return View(returnItem);
        }

        // POST: ReturnItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var returnItem = await _context.ReturnItem.SingleOrDefaultAsync(m => m.ReturnItemID == id);
            _context.ReturnItem.Remove(returnItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReturnItemExists(int id)
        {
            return _context.ReturnItem.Any(e => e.ReturnItemID == id);
        }
    }
}
