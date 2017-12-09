using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MobileStore.Data;
using MobileStore.Models;
using MobileStore.Models.StockReceivingViewModels;

namespace MobileStore.Controllers
{
    public class ModelFromSuppliersController : Controller
    {
        // GET: Items/Delete/5
        public async Task<IActionResult> DeleteItem(int? id)
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
        [HttpPost, ActionName("DeleteItem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedItem(int id)
        {
            var item = await _context.Item.SingleOrDefaultAsync(m => m.ItemID == id);
            _context.Item.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit","ModelFromSuppliers",new{id=item.ModelFromSupplierID});
        }
        // GET: Items/Edit/5
        public async Task<IActionResult> EditItem(int? id)
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
            var listModelFromSupplier = _context.ModelFromSupplier.Select(s=>new
            {
                s.ModelFromSupplierID,
                DisplayName = String.Format("{0} từ {1} ngày {2}", s.Model.Name, s.Supplier.Name, s.GetDate())

            });
            
            //    listModelFromSupplier.Select(s => new
            //{
            //    s.ModelFromSupplierID,
            //    Name = s.GetStockRecevingName()
            //});
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "Name", item.ModelID);

            ViewData["ModelFromSupplierID"] = new SelectList(listModelFromSupplier, "ModelFromSupplierID", "DisplayName",item.ModelFromSupplierID);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(int id, [Bind("ItemID,Name,IMEI,SerializerNumber,Note,Status,ModelFromSupplierID,ModelID")] Item item)
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
                return RedirectToAction("Edit", new { id = item.ModelFromSupplierID });
            }
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "ModelID", item.ModelID);
            ViewData["ModelFromSupplierID"] = new SelectList(_context.ModelFromSupplier, "ModelFromSupplierID", "ModelFromSupplierID", item.ModelFromSupplierID);
            return View(item);
        }
        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.ItemID == id);
        }

        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ModelFromSuppliersController(ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;

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
                //var model = _context.Model.Single(m => m.ModelID == modelFromSupplier.ModelID);
                //var supplier = _context.Supplier.Single(s => s.SupplierID == modelFromSupplier.SupplierID);
                //modelFromSupplier.Model = model;
                //modelFromSupplier.Supplier = supplier;
                modelFromSupplier.ApplicationUserID = _userManager.GetUserId(User);
                _context.Add(modelFromSupplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "Name", modelFromSupplier.ModelID);
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "Name", modelFromSupplier.SupplierID);
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
                return RedirectToAction(nameof(Edit),new{id=stockReceivingVM.ModelFromSupplier.ModelFromSupplierID});
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
            //var items = await _context.Item.Where(i => i.ModelFromSupplierID == id).ToListAsync();
            //_context.Item.RemoveRange(items);

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
