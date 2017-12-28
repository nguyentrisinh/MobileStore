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
    public class WarrantyDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WarrantyDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WarrantyDetails
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WarrantyDetail.Include(w => w.WarrantyCard);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: WarrantyDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warrantyDetail = await _context.WarrantyDetail
                .Include(w => w.WarrantyCard)
                .SingleOrDefaultAsync(m => m.WarrantyDetailID == id);
            if (warrantyDetail == null)
            {
                return NotFound();
            }

            return View(warrantyDetail);
        }

        // GET: WarrantyDetails/Create
        public IActionResult Create()
        {
            ViewData["WarrantyCardID"] = new SelectList(_context.WarrantyCard, "WarrantyCardID", "NumberOfWarranty");
            return View();
        }

        // POST: WarrantyDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WarrantyDetailID,Date,DefectInfo,Status,WarrantyCardID")] WarrantyDetail warrantyDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(warrantyDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WarrantyCardID"] = new SelectList(_context.WarrantyCard, "WarrantyCardID", "NumberOfWarranty", warrantyDetail.WarrantyCardID);
            return View(warrantyDetail);
        }

        // GET: WarrantyDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warrantyDetail = await _context.WarrantyDetail.SingleOrDefaultAsync(m => m.WarrantyDetailID == id);
            if (warrantyDetail == null)
            {
                return NotFound();
            }
            ViewData["WarrantyCardID"] = new SelectList(_context.WarrantyCard, "WarrantyCardID", "NumberOfWarranty", warrantyDetail.WarrantyCardID);
            return View(warrantyDetail);
        }

        // POST: WarrantyDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WarrantyDetailID,Date,DefectInfo,Status,WarrantyCardID")] WarrantyDetail warrantyDetail)
        {
            if (id != warrantyDetail.WarrantyDetailID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(warrantyDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarrantyDetailExists(warrantyDetail.WarrantyDetailID))
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
            ViewData["WarrantyCardID"] = new SelectList(_context.WarrantyCard, "WarrantyCardID", "NumberOfWarranty", warrantyDetail.WarrantyCardID);
            return View(warrantyDetail);
        }

        // GET: WarrantyDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warrantyDetail = await _context.WarrantyDetail
                .Include(w => w.WarrantyCard)
                .SingleOrDefaultAsync(m => m.WarrantyDetailID == id);
            if (warrantyDetail == null)
            {
                return NotFound();
            }

            return View(warrantyDetail);
        }

        // POST: WarrantyDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var warrantyDetail = await _context.WarrantyDetail.SingleOrDefaultAsync(m => m.WarrantyDetailID == id);
            _context.WarrantyDetail.Remove(warrantyDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WarrantyDetailExists(int id)
        {
            return _context.WarrantyDetail.Any(e => e.WarrantyDetailID == id);
        }
    }
}
