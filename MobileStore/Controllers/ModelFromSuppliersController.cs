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
    public class ModelFromSuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModelFromSuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ModelFromSuppliers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ModelFromSupplier.Include(m => m.Model).Include(m => m.Supplier);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ModelFromSuppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelFromSupplier = await _context.ModelFromSupplier
                .Include(m => m.Model)
                .Include(m => m.Supplier)
                .SingleOrDefaultAsync(m => m.ModelFromSupplierID == id);
            if (modelFromSupplier == null)
            {
                return NotFound();
            }

            return View(modelFromSupplier);
        }

        // GET: ModelFromSuppliers/Create
        public IActionResult Create()
        {
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "Name");
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "Name");
            return View();
        }

        // POST: ModelFromSuppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModelFromSupplierID,Quantity,PriceBought,PriceSold,Date,SupplierID,ModelID")] ModelFromSupplier modelFromSupplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modelFromSupplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "ModelID", modelFromSupplier.ModelID);
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "SupplierID", modelFromSupplier.SupplierID);
            return View(modelFromSupplier);
        }

        // GET: ModelFromSuppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var listItems = await _context.Item.Where(i => i.ModelFromSupplierID == id).ToListAsync();
            var modelFromSupplier = await _context.ModelFromSupplier.SingleOrDefaultAsync(m => m.ModelFromSupplierID == id);
            if (modelFromSupplier == null)
            {
                return NotFound();
            }
            ViewData["ModelItemID"] = modelFromSupplier.ModelID;
            ViewData["ModelFromSupplierID"] = modelFromSupplier.ModelFromSupplierID;
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "Name", modelFromSupplier.ModelID);
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "Name", modelFromSupplier.SupplierID);
            var stockReceivingVM =new StockReceivingViewModel();
            stockReceivingVM.ModelFromSupplier = modelFromSupplier;
            stockReceivingVM.Items = listItems;
          
            return View(stockReceivingVM);
        }

        // POST: ModelFromSuppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StockReceivingViewModel stockReceivingVM)
        {
            if (id != stockReceivingVM.ModelFromSupplier.ModelFromSupplierID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //var modelFromSupplier = new ModelFromSupplier
                    //{
                    //    ModelFromSupplierID = 
                    //    Date = stockReceivingVM.ModelFromSupplier.Date,
                    //    Quantity = stockReceivingVM.ModelFromSupplier.Quantity,
                    //    PriceBought =  stockReceivingVM.ModelFromSupplier.PriceBought,
                    //    PriceSold = stockReceivingVM.ModelFromSupplier.PriceSold,
                    //    SupplierID = stockReceivingVM.ModelFromSupplier.SupplierID,
                    //    ModelID = stockReceivingVM.ModelFromSupplier.ModelID
                    //};
                    _context.Update(stockReceivingVM.ModelFromSupplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelFromSupplierExists(stockReceivingVM.ModelFromSupplier.ModelFromSupplierID))
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
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "ModelID", stockReceivingVM.ModelFromSupplier.ModelID);
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "SupplierID", stockReceivingVM.ModelFromSupplier.SupplierID);
           
            return View(stockReceivingVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(StockReceivingViewModel stockReceivingVM)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stockReceivingVM.Item);
                await _context.SaveChangesAsync();
            }
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "ModelID", stockReceivingVM.ModelFromSupplier.ModelID);
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "SupplierID", stockReceivingVM.ModelFromSupplier.SupplierID);

            return RedirectToAction("Edit","ModelFromSuppliers",new {id = stockReceivingVM.ModelFromSupplier.ModelFromSupplierID});
        }

        // GET: ModelFromSuppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelFromSupplier = await _context.ModelFromSupplier
                .Include(m => m.Model)
                .Include(m => m.Supplier)
                .SingleOrDefaultAsync(m => m.ModelFromSupplierID == id);
            if (modelFromSupplier == null)
            {
                return NotFound();
            }

            return View(modelFromSupplier);
        }

        // POST: ModelFromSuppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modelFromSupplier = await _context.ModelFromSupplier.SingleOrDefaultAsync(m => m.ModelFromSupplierID == id);
            _context.ModelFromSupplier.Remove(modelFromSupplier);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModelFromSupplierExists(int id)
        {
            return _context.ModelFromSupplier.Any(e => e.ModelFromSupplierID == id);
        }
    }
}
