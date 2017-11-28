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
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        [HttpPost]
        public string Index(string searchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + searchString;
        }
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["IDSortParm"] = sortOrder == "ID" ? "id_desc" : "ID";

            ViewData["PicNameSortParm"] = sortOrder == "PicName" ? "picname_desc" : "PicName";

            ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";
            ViewData["CurrentFilter"] = searchString;
            var suppliers = from m in _context.Supplier
                select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                suppliers = suppliers.Where(s => s.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    suppliers = suppliers.OrderByDescending(s => s.Name);
                    break;
                case "ID":
                    suppliers = suppliers.OrderBy(s => s.SupplierID);
                    break;
                case "id_desc":
                    suppliers = suppliers.OrderByDescending(s => s.SupplierID);
                    break;
                case "PicName":
                    suppliers = suppliers.OrderBy(s => s.PicName);
                    break;
                case "picname_desc":
                    suppliers = suppliers.OrderByDescending(s => s.PicName);
                    break;
                case "Email":
                    suppliers = suppliers.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    suppliers = suppliers.OrderByDescending(s => s.Email);
                    break;
                default:
                    suppliers = suppliers.OrderBy(s => s.Name);
                    break;
            }

            return View(await suppliers.ToListAsync());
        }
        //public async Task<IActionResult> Index()
        //{

        //    return View(await _context.Supplier.ToListAsync());
        //}

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier
                .SingleOrDefaultAsync(m => m.SupplierID == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SupplierID,Name,Address,Phone,Email,Code,Status,PicName,PicPhone,PicEmail")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier.SingleOrDefaultAsync(m => m.SupplierID == id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SupplierID,Name,Address,Phone,Email,Code,Status,PicName,PicPhone,PicEmail")] Supplier supplier)
        {
            if (id != supplier.SupplierID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.SupplierID))
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
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier
                .SingleOrDefaultAsync(m => m.SupplierID == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _context.Supplier.SingleOrDefaultAsync(m => m.SupplierID == id);
            _context.Supplier.Remove(supplier);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return _context.Supplier.Any(e => e.SupplierID == id);
        }
    }
}
