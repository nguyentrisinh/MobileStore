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
using Remotion.Linq.Clauses;

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
                stockReceivings = stockReceivings.Include(m => m.ApplicationUser).Include(m => m.Supplier).Where(m => m.StockReceivingID.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "date_desc":
                    stockReceivings = stockReceivings.OrderByDescending(s => s.Date).Include(m => m.ApplicationUser).Include(m => m.Supplier);
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
            int pageSize = 12;

            return View(await PaginatedList<StockReceiving>.CreateAsync(stockReceivings.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: StockReceivings/Details/5
        public async Task<IActionResult> Details(Guid? id, string sortOrder, string currentFilter, string searchString, int? page)
        {
            #region Check Exist

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
            #endregion
            #region Filter and Search
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date" : "";
            ViewData["QuantitySortParm"] = sortOrder == "quantity" ? "quantity_desc" : "quantity";
            ViewData["PriceBoughtSortParm"] = sortOrder == "pricebought" ? "pricebought_desc" : "pricebought";
            ViewData["PriceSoldSortParm"] = sortOrder == "pricesold" ? "pricesold_desc" : "pricesold";
            ViewData["ModelSortParm"] = sortOrder == "model" ? "model_desc" : "model";
            ViewData["PeriodSortParm"] = sortOrder == "period" ? "period_desc" : "period";

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

            var modelsFromSuppliers = _context.ModelFromSupplier.Where(m => m.StockReceivingID == id);

            if (!String.IsNullOrEmpty(searchString))
            {
                modelsFromSuppliers = modelsFromSuppliers.Include(m => m.Model).Where(m => m.Model.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "date":
                    modelsFromSuppliers = modelsFromSuppliers.OrderBy(s => s.Date).Include(m => m.Model);
                    break;
                case "quantity":
                    modelsFromSuppliers = modelsFromSuppliers.OrderBy(s => s.Quantity).Include(m => m.Model);
                    break;
                case "quantity_desc":
                    modelsFromSuppliers = modelsFromSuppliers.OrderByDescending(s => s.Quantity).Include(m => m.Model);
                    break;
                case "period":
                    modelsFromSuppliers = modelsFromSuppliers.OrderBy(s => s.Period).Include(m => m.Model);
                    break;
                case "period_desc":
                    modelsFromSuppliers = modelsFromSuppliers.OrderByDescending(s => s.Period).Include(m => m.Model);
                    break;

                case "pricebought":
                    modelsFromSuppliers = modelsFromSuppliers.OrderBy(s => s.PriceBought).Include(m => m.Model);
                    break;
                case "pricebought_desc":
                    modelsFromSuppliers = modelsFromSuppliers.OrderByDescending(s => s.PriceBought).Include(m => m.Model);
                    break;
                case "pricesold":
                    modelsFromSuppliers = modelsFromSuppliers.OrderBy(s => s.PriceSold).Include(m => m.Model);
                    break;
                case "pricesold_desc":
                    modelsFromSuppliers = modelsFromSuppliers.OrderByDescending(s => s.PriceSold).Include(m => m.Model);
                    break;
                case "model":
                    modelsFromSuppliers = modelsFromSuppliers.OrderBy(s => s.Model.Name).Include(m => m.Model);
                    break;
                case "model_desc":
                    modelsFromSuppliers = modelsFromSuppliers.OrderByDescending(s => s.Model.Name).Include(m => m.Model);
                    break;
                default:
                    modelsFromSuppliers = modelsFromSuppliers.OrderByDescending(s => s.Date).Include(m => m.Model);
                    break;
            }
            #endregion
            #region Logic
            var stockReceivingVm = new StockReceivingViewModel();
            stockReceivingVm.StockReceiving = stockReceiving;
            stockReceivingVm.Models = _context.Model;
            stockReceivingVm.Suppliers = _context.Supplier;
            #endregion
            #region Paging
            int pageSize = 6;
            PaginatedList<ModelFromSupplier> pagesModelsFromSuppliers = await PaginatedList<ModelFromSupplier>.CreateAsync(modelsFromSuppliers.AsNoTracking(), page ?? 1, pageSize);
            stockReceivingVm.ModelFromSuppliers = pagesModelsFromSuppliers;
            #endregion
            return View(stockReceivingVm);
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
                return RedirectToAction(nameof(Details), new { id = stockReceiving.StockReceivingID });
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
            var timeSpan = DateTime.Now - stockReceiving.Date;
            if (timeSpan.Hours > 2)
            {
                ViewData["ErrorText"] = "Không thể sửa đơn nhập hàng sau 2 giờ";
                return View("ErrorPage");
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, stockReceiving,
                OrderOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            ViewData["SupplierID"] = _context.Supplier;
            return View(stockReceiving);
        }

        // POST: StockReceivings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Edit(Guid id, StockReceiving stockReceiving)
        {
            if (id != stockReceiving.StockReceivingID)
            {
                return NotFound();
            }



            if (ModelState.IsValid)
            {
                try
                {
                    var item = MakeNewStockReceiving(stockReceiving).Result;
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
                    if (!StockReceivingExists(stockReceiving.StockReceivingID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(Details), new { id = stockReceiving.StockReceivingID });
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
            var modelFromSuppliers = await _context.ModelFromSupplier.Where(m => m.StockReceivingID == id).Include(m => m.Model).ToListAsync();
            var stockReceivingVM = new StockReceivingViewModel();
            stockReceivingVM.StockReceiving = stockReceiving;
            stockReceivingVM.ModelFromSuppliers = modelFromSuppliers;
            stockReceivingVM.Models = _context.Model;
            stockReceivingVM.Suppliers = _context.Supplier;

            return View(stockReceivingVM);
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
                stockReceivingVM.ModelFromSupplier.Quantity = 0;
                _context.Add(stockReceivingVM.ModelFromSupplier);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = stockReceivingVM.ModelFromSupplier.StockReceivingID });
        }






        private bool StockReceivingExists(Guid id)
        {
            return _context.StockReceiving.Any(e => e.StockReceivingID == id);
        }

        private async Task<StockReceiving> MakeNewStockReceiving(StockReceiving stockReceiving)
        {
            var newStockReceiving = await _context.StockReceiving
                .Where(m => m.StockReceivingID == stockReceiving.StockReceivingID).Include(m => m.Supplier).SingleOrDefaultAsync();
            newStockReceiving.SupplierID = stockReceiving.SupplierID;
            return newStockReceiving;
        }
    }
}
