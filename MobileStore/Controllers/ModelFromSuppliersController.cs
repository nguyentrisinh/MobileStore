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
using MobileStore.Authorization;
using MobileStore.Data;
using MobileStore.Models;
using MobileStore.Models.StockReceivingViewModels;

namespace MobileStore.Controllers
{
    public class ModelFromSuppliersController : Controller
    {
        [Authorize(Roles = "WarehouseManager, Admin")]
        // GET: Items/Delete/5
        public async Task<IActionResult> DeleteItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(i => i.Model)
                .Include(m=>m.ModelFromSupplier)
                .ThenInclude(m=>m.StockReceiving)
                .SingleOrDefaultAsync(m => m.ItemID == id);

            if (item == null)
            {
                return NotFound();
            }

            var timeSpan = DateTime.Now - item.ModelFromSupplier.StockReceiving.Date;
            if (timeSpan.Hours > 2)
            {
                ViewData["ErrorText"] = "Không thể xóa sản phẩm sau 2 giờ";
                return View("ErrorPage");
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, item.ModelFromSupplier.StockReceiving,
                OrderOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("DeleteItem")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> DeleteConfirmedItem(int id)
        {
            var item = await _context.Item
                .Include(m=>m.ModelFromSupplier)
                .ThenInclude(m => m.StockReceiving)
                .SingleOrDefaultAsync(m => m.ItemID == id);
            var timeSpan = DateTime.Now - item.ModelFromSupplier.StockReceiving.Date;
            if (timeSpan.Hours > 2)
            {
                ViewData["ErrorText"] = "Không thể xóa sản phẩm sau 2 giờ";
                return View("ErrorPage");
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, item.ModelFromSupplier.StockReceiving,
                OrderOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            _context.Item.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit","ModelFromSuppliers",new{id=item.ModelFromSupplierID});
        }
        // GET: Items/Edit/5
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> EditItem(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .Include(m=>m.ModelFromSupplier)
                .ThenInclude(m => m.StockReceiving)
                .SingleOrDefaultAsync(m => m.ItemID == id);

            if (item == null)
            {
                return NotFound();
            }

            var timeSpan = DateTime.Now - item.ModelFromSupplier.StockReceiving.Date;
            if (timeSpan.Hours > 2)
            {
                ViewData["ErrorText"] = "Không thể xóa sản phẩm sau 2 giờ";
                return View("ErrorPage");
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, item.ModelFromSupplier.StockReceiving,
                OrderOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            var listModelFromSupplier = _context.ModelFromSupplier.Select(s=>new
            {
                s.ModelFromSupplierID,
                DisplayName = String.Format("{0} từ {1} ngày {2}", s.Model.Name, s.StockReceiving.Supplier.Name, s.GetDate())

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
        [Authorize(Roles = "WarehouseManager, Admin")]
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
                    var newItem = ViewModelToItem(item).Result;
                    var timeSpan = DateTime.Now - newItem.ModelFromSupplier.StockReceiving.Date;
                    if (timeSpan.Hours > 2)
                    {
                        ViewData["ErrorText"] = "Không thể xóa sản phẩm sau 2 giờ";
                        return View("ErrorPage");
                    }
                    var isAuthorized = await _authorizationService.AuthorizeAsync(User, newItem.ModelFromSupplier.StockReceiving,
                        OrderOperations.Update);
                    if (!isAuthorized.Succeeded)
                    {
                        return new ChallengeResult();
                    }

                    _context.Update(newItem);
                    
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
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["ModelSortParm"] = sortOrder == "model" ? "model_desc" : "model";
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

            var modelFromSuppliers = from m in _context.ModelFromSupplier select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                modelFromSuppliers = modelFromSuppliers.Include(m => m.Model).Where(s => s.ModelFromSupplierID.ToString()==searchString);
            }
            switch (sortOrder)
            {
                case "date_desc":
                    modelFromSuppliers = modelFromSuppliers.OrderByDescending(s => s.Date).Include(m => m.Model);
                    break;
                case "model":
                    modelFromSuppliers = modelFromSuppliers.OrderBy(s => s.Model.Name).Include(m => m.Model);
                    break;
                case "model_desc":
                    modelFromSuppliers = modelFromSuppliers.OrderByDescending(s => s.Model.Name).Include(m => m.Model);
                    break;
               default:
                    modelFromSuppliers = modelFromSuppliers.OrderBy(s => s.Date).Include(m => m.Model);
                    break;
            }
            int pageSize = 1;

            return View(await PaginatedList<ModelFromSupplier>.CreateAsync(modelFromSuppliers.AsNoTracking(), page ?? 1, pageSize));
           
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
                .SingleOrDefaultAsync(m => m.ModelFromSupplierID == id);
            if (modelFromSupplier == null)
            {
                return NotFound();
            }

            return View(modelFromSupplier);
        }

        // GET: ModelFromSuppliers/Edit/5
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.ModelFromSupplier.Include(m=>m.StockReceiving).SingleOrDefaultAsync(m => m.ModelFromSupplierID == id);

            if (item == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, item.StockReceiving,
                OrderOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            var listItems = await _context.Item.Where(i => i.ModelFromSupplierID == id).ToListAsync();
            ViewData["ModelItemID"] = item.ModelID;
            ViewData["ModelFromSupplierID"] = item.ModelFromSupplierID;
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "Name", item.ModelID);
            ViewData["StockReceivingID"] = new SelectList(_context.StockReceiving, "StockReceivingID", "StockReceivingID", item.StockReceivingID);
            var stockReceivingDetailVM =new StockReceivingDetailViewModel();
            stockReceivingDetailVM.ModelFromSupplier = item;
            stockReceivingDetailVM.Items = listItems;
          
            return View(stockReceivingDetailVM);
        }

        // POST: ModelFromSuppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Edit(int id, StockReceivingViewModel stockReceivingVM)
        {
            try
            {
                var item = ViewModelToModelFromSupplier(stockReceivingVM).Result;
                var timeSpan = DateTime.Now - item.StockReceiving.Date;
                if (timeSpan.Hours > 2)
                {
                    ViewData["ErrorText"] = "Không thể xóa sản phẩm sau 2 giờ";
                    return View("ErrorPage");
                }
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, item.StockReceiving,
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
                if (!ModelFromSupplierExists(stockReceivingVM.ModelFromSupplier.ModelFromSupplierID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Edit), new { id = stockReceivingVM.ModelFromSupplier.ModelFromSupplierID });

            //ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "ModelID", stockReceivingVM.ModelFromSupplier.ModelID);ModelFromSupplier.StockReceiving
            //ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "SupplierID", stockReceivingVM.ModelFromSupplier.SupplierID);

            //return View(stockReceivingVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> CreateItem(StockReceivingDetailViewModel stockReceivingDetailVM)
        {
            if (ModelState.IsValid)
            {
                var modelFromSupplier = await _context.ModelFromSupplier.Include(m=>m.StockReceiving)
                    .SingleOrDefaultAsync(m => m.ModelFromSupplierID == stockReceivingDetailVM.Item.ModelFromSupplierID);
                    
                var timeSpan = DateTime.Now - modelFromSupplier.StockReceiving.Date;
                if (timeSpan.Hours > 2)
                {
                    ViewData["ErrorText"] = "Không thể tạo sản phẩm sau 2 giờ";
                    return View("ErrorPage");
                }
                _context.Add(stockReceivingDetailVM.Item);
                await _context.SaveChangesAsync();
            }
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "ModelID", stockReceivingDetailVM.ModelFromSupplier.ModelID);
            //ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "SupplierID", stockReceivingDetailVM.ModelFromSupplier.SupplierID);

            return RedirectToAction("Edit","ModelFromSuppliers",new {id = stockReceivingDetailVM.ModelFromSupplier.ModelFromSupplierID});
        }

        // GET: ModelFromSuppliers/Delete/5
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelFromSupplier = await _context.ModelFromSupplier
                .Include(m => m.Model).Include(m=>m.StockReceiving)
                .SingleOrDefaultAsync(m => m.ModelFromSupplierID == id);
            if (modelFromSupplier == null)
            {
                return NotFound();
            }

            var timeSpan = DateTime.Now - modelFromSupplier.StockReceiving.Date;
            if (timeSpan.Hours > 2)
            {
                ViewData["ErrorText"] = "Không thể xóa sản phẩm sau 2 giờ";
                return View("ErrorPage");
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, modelFromSupplier.StockReceiving,
                OrderOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }


            return View(modelFromSupplier);
        }

        // POST: ModelFromSuppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var items = await _context.Item.Where(i => i.ModelFromSupplierID == id).ToListAsync();
            //_context.Item.RemoveRange(items);

            var modelFromSupplier = await _context.ModelFromSupplier.Include(m=>m.StockReceiving).SingleOrDefaultAsync(m => m.ModelFromSupplierID == id);

            var timeSpan = DateTime.Now - modelFromSupplier.StockReceiving.Date;
            if (timeSpan.Hours > 2)
            {
                ViewData["ErrorText"] = "Không thể xóa sản phẩm sau 2 giờ";
                return View("ErrorPage");
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, modelFromSupplier.StockReceiving,
                OrderOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            _context.ModelFromSupplier.Remove(modelFromSupplier);

            await _context.SaveChangesAsync();
            return RedirectToAction("Edit","StockReceivings",new {id = modelFromSupplier.StockReceivingID});
        }
        #region helper

        private bool ModelFromSupplierExists(int id)
        {
            return _context.ModelFromSupplier.Any(e => e.ModelFromSupplierID == id);
        }

        public async Task<ModelFromSupplier> ViewModelToModelFromSupplier(StockReceivingViewModel stockReceivingViewModel)
        {
            var item = await _context.ModelFromSupplier.Include(m=>m.StockReceiving).SingleOrDefaultAsync(m =>
                m.ModelFromSupplierID == stockReceivingViewModel.ModelFromSupplier.ModelFromSupplierID);
            item.ModelFromSupplierID = stockReceivingViewModel.ModelFromSupplier.ModelFromSupplierID;
            item.ModelID = stockReceivingViewModel.ModelFromSupplier.ModelID;
            item.PriceBought = stockReceivingViewModel.ModelFromSupplier.PriceBought;
            item.PriceSold = stockReceivingViewModel.ModelFromSupplier.PriceSold;
            item.Quantity = stockReceivingViewModel.ModelFromSupplier.Quantity;
            item.Period = stockReceivingViewModel.ModelFromSupplier.Period;
            item.StockReceivingID = stockReceivingViewModel.ModelFromSupplier.StockReceivingID;
            return item;
        }

        public async Task<Item> ViewModelToItem(Item item)
        {
            var newItem = await _context.Item.Include(m=>m.ModelFromSupplier)
                .ThenInclude(m => m.StockReceiving).SingleOrDefaultAsync(m =>
                m.ItemID == item.ItemID);
            newItem.ItemID = item.ItemID;
            newItem.IMEI = item.IMEI;
            newItem.ModelFromSupplierID = item.ModelFromSupplierID;
            newItem.ModelID = item.ModelID;
            newItem.Name = item.Name;
            newItem.Note = item.Note;
            newItem.SerializerNumber = item.SerializerNumber;
            newItem.Status = item.Status;
            return newItem;
        }
#endregion

    }
}
