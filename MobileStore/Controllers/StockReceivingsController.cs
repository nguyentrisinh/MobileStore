using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MobileStore.Authorization;
using MobileStore.Data;
using MobileStore.Models;
using MobileStore.Models.StockReceivingViewModels;

namespace MobileStore.Controllers
{
    public class StockReceivingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public StockReceivingsController(ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        // GET: StockReceivings
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["SupplierSortParm"] = sortOrder == "supplier" ? "supplier_desc" : "supplier";
            ViewData["StaffSortParm"] = sortOrder == "staff" ? "staff_desc" : "staff";

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

            var stockReceivings = from m in _context.StockReceiving select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                    stockReceivings = stockReceivings.Include(m => m.ApplicationUser).Include(m => m.Supplier).Where(m=>m.StockReceivingID.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "date_desc":
                    stockReceivings = stockReceivings.OrderByDescending(s => s.Date).Include(m => m.ApplicationUser).Include(m => m.Supplier);
                    break;
                case "supplier":
                    stockReceivings = stockReceivings.OrderBy(s => s.Supplier.Name).Include(m => m.ApplicationUser).Include(m => m.Supplier);
                    break;
                case "supplier_desc":
                    stockReceivings = stockReceivings.OrderByDescending(s => s.Supplier.Name).Include(m => m.ApplicationUser).Include(m => m.Supplier);
                    break;
       
                case "staff":
                    stockReceivings = stockReceivings.OrderBy(s => s.ApplicationUser.FirstName).Include(m => m.ApplicationUser).Include(m => m.Supplier);
                    break;
                case "staff_desc":
                    stockReceivings = stockReceivings.OrderByDescending(s => s.ApplicationUser.FirstName).Include(m => m.ApplicationUser).Include(m => m.Supplier);
                    break;
                default:
                    stockReceivings = stockReceivings.OrderBy(s => s.Date).Include(m => m.ApplicationUser).Include(m => m.Supplier);
                    break;
            }
            int pageSize = 1;

            return View(await PaginatedList<StockReceiving>.CreateAsync(stockReceivings.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: StockReceivings/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockReceiving = await _context.StockReceiving
                .Include(s => s.ApplicationUser)
                .Include(s => s.Supplier)
                .SingleOrDefaultAsync(m => m.StockReceivingID == id);
            if (stockReceiving == null)
            {
                return NotFound();
            }

            return View(stockReceiving);
        }

        // GET: StockReceivings/Create

        [Authorize(Roles = "WarehouseManager, Admin")]
        public IActionResult Create()
        {
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "Name");
            return View();
        }

        // POST: StockReceivings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Create([Bind("StockReceivingID,Date,SupplierID,ApplicationUserID")] StockReceiving stockReceiving)
        {
            if (ModelState.IsValid)
            {
                stockReceiving.ApplicationUserID = _userManager.GetUserId(User);
                stockReceiving.Date = DateTime.Now;
                stockReceiving.StockReceivingID = Guid.NewGuid();
                _context.Add(stockReceiving);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "Name", stockReceiving.SupplierID);
            return View(stockReceiving);
        }

        // GET: StockReceivings/Edit/5
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockReceiving = await _context.StockReceiving.SingleOrDefaultAsync(m => m.StockReceivingID == id);

            if (stockReceiving == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, stockReceiving,
                OrderOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            var modelFromSuppliers = await _context.ModelFromSupplier.Where(m => m.StockReceivingID == id).Include(m => m.Model).ToListAsync();
            var stockReceivingVM = new StockReceivingViewModel();
            stockReceivingVM.StockReceiving = stockReceiving;
            stockReceivingVM.ModelFromSuppliers = modelFromSuppliers;
            stockReceivingVM.Models = _context.Model;
            stockReceivingVM.Suppliers = _context.Supplier;
            return View(stockReceivingVM);
        }

        // POST: StockReceivings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Edit(Guid id, StockReceivingViewModel stockReceivingVM)
        {
            if (id != stockReceivingVM.StockReceiving.StockReceivingID)
            {
                return NotFound();
            }



            if (ModelState.IsValid)
            {
                try
                {
                    var item = MakeNewStockReceiving(stockReceivingVM).Result;
                    var timeSpan = DateTime.Now - item.Date;
                    if (timeSpan.Hours > 2)
                    {
                        ViewData["ErrorText"] = "Không thể sửa đơn nhập hàng sau 2 giờ";
                        return View("ErrorPage");
                    }
                    var isAuthorized = await _authorizationService.AuthorizeAsync(User, item,
                        OrderOperations.Update);
                    if (!isAuthorized.Succeeded)
                    {
                        return new ChallengeResult();
                    }
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockReceivingExists(stockReceivingVM.StockReceiving.StockReceivingID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(Edit), new { id = stockReceivingVM.StockReceiving.StockReceivingID });
        }

        // GET: StockReceivings/Delete/5
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockReceiving = await _context.StockReceiving
                .Include(s => s.ApplicationUser)
                .Include(s => s.Supplier)
                .SingleOrDefaultAsync(m => m.StockReceivingID == id);
            if (stockReceiving == null)
            {
                return NotFound();
            }

            var timeSpan = DateTime.Now - stockReceiving.Date;
            if (timeSpan.Hours > 2)
            {
                ViewData["ErrorText"] = "Không thể xóa xóa đơn nhập hàng sau 2 giờ";
                return View("ErrorPage");
            }
            var hasModelFromSupplier = await _context.ModelFromSupplier.Where(m => m.StockReceivingID == id).AnyAsync();
            if (hasModelFromSupplier)
            {
                ViewData["ErrorText"] = "Không thể xóa do đã có do đã nhập chi tiết đơn hàng bán ra ";
                return View("ErrorPage");
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, stockReceiving,
                OrderOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return View(stockReceiving);
        }

        // POST: StockReceivings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var stockReceiving = await _context.StockReceiving.SingleOrDefaultAsync(m => m.StockReceivingID == id);

            var timeSpan = DateTime.Now - stockReceiving.Date;
            if (timeSpan.Hours > 2)
            {
                ViewData["ErrorText"] = "Không thể xóa đơn nhập hàng sau 2 giờ";
                return View("ErrorPage");
            }
            var hasModelFromSupplier = await _context.ModelFromSupplier.Where(m => m.StockReceivingID == id).AnyAsync();
            if (hasModelFromSupplier)
            {
                ViewData["ErrorText"] = "Không thể xóa do đã có do đã nhập chi tiết đơn hàng bán ra ";
                return View("ErrorPage");
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, stockReceiving,
                OrderOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            _context.StockReceiving.Remove(stockReceiving);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> CreateModelFromSupplier(StockReceivingViewModel stockReceivingVM)
        {
            if (ModelState.IsValid)
            {
                var stockReceiving = await _context.StockReceiving.SingleAsync(m => m.StockReceivingID == stockReceivingVM.ModelFromSupplier.StockReceivingID);

                
                var timeSpan = DateTime.Now - stockReceiving.Date;
                if (timeSpan.Hours > 2)
                {
                    ViewData["ErrorText"] = "Bạn không thể sửa sau 2h";
                    return View("ErrorPage");
                }
                stockReceivingVM.ModelFromSupplier.Date = DateTime.Now;
                _context.Add(stockReceivingVM.ModelFromSupplier);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), new { id = stockReceivingVM.ModelFromSupplier.StockReceivingID });
        }






        private bool StockReceivingExists(Guid id)
        {
            return _context.StockReceiving.Any(e => e.StockReceivingID == id);
        }

        private async Task<StockReceiving> MakeNewStockReceiving(StockReceivingViewModel stockReceivingVM)
        {
            var newStockReceiving = await _context.StockReceiving
                .Where(m => m.StockReceivingID == stockReceivingVM.StockReceiving.StockReceivingID).Include(m=>m.Supplier).SingleOrDefaultAsync();
            newStockReceiving.SupplierID = stockReceivingVM.StockReceiving.SupplierID;
            return newStockReceiving;
        }
    }
}
