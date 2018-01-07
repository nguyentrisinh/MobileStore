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
            item.ModelFromSupplier.Quantity -= 1;
            _context.Update(item.ModelFromSupplier);
            _context.Item.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details","ModelFromSuppliers",new{id=item.ModelFromSupplierID});
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
                return RedirectToAction("Details", new { id = item.ModelFromSupplierID });
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
            int pageSize = 12;

            return View(await PaginatedList<ModelFromSupplier>.CreateAsync(modelFromSuppliers.AsNoTracking(), page ?? 1, pageSize));
           
        }

        // GET: ModelFromSuppliers/Details/5
        public async Task<IActionResult> Details(int? id, string sortOrder, string currentFilter, string searchString, int? page)
        {
            #region Check Exists
            if (id == null)
            {
                return NotFound();
            }
            var item = await _context.ModelFromSupplier.Include(m => m.StockReceiving).Include(m=>m.Model).SingleOrDefaultAsync(m => m.ModelFromSupplierID == id);

           
            if (item == null)
            {
                return NotFound();
            }
            #endregion

            #region Filter and Search
            ViewData["ModelSortParm"] = String.IsNullOrEmpty(sortOrder) ? "model_desc" : "";
            ViewData["StatusSortParm"] = sortOrder == "status" ? "status_desc" : "status";
            ViewData["SerializerNumberSortParm"] = sortOrder == "serializer_number" ? "serializer_number_desc" : "serializer_number";
            ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";

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

            var items = _context.Item.Include(m=>m.Model).Where(m => m.ModelFromSupplierID == id);

            if (!String.IsNullOrEmpty(searchString))
            {
                items = items.Include(m=>m.Model).Where(m => m.SerializerNumber.ToString().ToLower().Contains(searchString.ToLower()) || m.IMEI.ToString().ToLower().Contains(searchString.ToLower()));
            }
            switch (sortOrder)
            {
                case "model_desc":
                    items = items.OrderByDescending(s => s.Model.Name).Include(m => m.Model);
                    break;
                case "status":
                    items = items.OrderBy(s => s.Status).Include(m => m.Model);
                    break;
                case "status_desc":
                    items = items.OrderByDescending(s => s.Status).Include(m => m.Model);
                    break;
                case "serializer_number":
                    items = items.OrderBy(s => s.SerializerNumber).Include(m => m.Model);
                    break;
                case "serializer_number_desc":
                    items = items.OrderByDescending(s => s.SerializerNumber).Include(m => m.Model);
                    break;
                case "name":
                    items = items.OrderBy(s => s.Name).Include(m => m.Model);
                    break;
                case "name_desc":
                    items = items.OrderByDescending(s => s.Name).Include(m => m.Model);
                    break;

                default:
                    items = items.OrderBy(s => s.Model.Name).Include(m => m.Model);
                    break;
            }
            #endregion
            ViewData["ModelItemID"] = item.ModelID;
            ViewData["ModelFromSupplierID"] = item.ModelFromSupplierID;
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "Name", item.ModelID);
            ViewData["StockReceivingID"] = new SelectList(_context.StockReceiving, "StockReceivingID", "StockReceivingID", item.StockReceivingID);
            var stockReceivingDetailVM = new StockReceivingDetailViewModel();
            stockReceivingDetailVM.ModelFromSupplier = item;

            #region Paging
            int pageSize = 6;
            PaginatedList<Item> pagesItems = await PaginatedList<Item>.CreateAsync(items.AsNoTracking(), page ?? 1, pageSize);
            stockReceivingDetailVM.Items = pagesItems;
            #endregion

            return View(stockReceivingDetailVM);
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
            //var listItems = await _context.Item.Where(i => i.ModelFromSupplierID == id).ToListAsync();
            //ViewData["ModelItemID"] = item.ModelID;
            //ViewData["ModelFromSupplierID"] = item.ModelFromSupplierID;
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "Name", item.ModelID);
            ViewData["StockReceivingID"] = new SelectList(_context.StockReceiving, "StockReceivingID", "StockReceivingID", item.StockReceivingID);
            //var stockReceivingDetailVM =new StockReceivingDetailViewModel();
            //stockReceivingDetailVM.ModelFromSupplier = item;
            //stockReceivingDetailVM.Items = listItems;

            return View(item);
        }

        // POST: ModelFromSuppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Edit(int id, ModelFromSupplier modelFromSupplier)
        {
            try
            {
                var item = ViewModelToModelFromSupplier(modelFromSupplier).Result;
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
                if (!ModelFromSupplierExists(modelFromSupplier.ModelFromSupplierID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Details), new { id = modelFromSupplier.ModelFromSupplierID });

            //ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "ModelID", stockReceivingVM.ModelFromSupplier.ModelID);ModelFromSupplier.StockReceiving
            //ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "SupplierID", stockReceivingVM.ModelFromSupplier.SupplierID);

            //return View(stockReceivingVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> CreateItem(StockReceivingDetailViewModel stockReceivingDetailVM)
        {

            var modelFromSupplier = await _context.ModelFromSupplier.Include(m => m.StockReceiving)
                .SingleOrDefaultAsync(m => m.ModelFromSupplierID == stockReceivingDetailVM.Item.ModelFromSupplierID);

            if (ModelState.IsValid)
            {
                var timeSpan = DateTime.Now - modelFromSupplier.StockReceiving.Date;
                if (timeSpan.Hours > 2)
                {
                    ViewData["ErrorText"] = "Không thể tạo sản phẩm sau 2 giờ";
                    return View("ErrorPage");
                }
                stockReceivingDetailVM.Item.Status = ItemStatus.InStock;
                modelFromSupplier.Quantity = modelFromSupplier.Quantity + 1;
                _context.Update(modelFromSupplier);
                _context.Add(stockReceivingDetailVM.Item);
                await _context.SaveChangesAsync();
            }
            //ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "SupplierID", stockReceivingDetailVM.ModelFromSupplier.SupplierID);

            return RedirectToAction("Details","ModelFromSuppliers",new {id = modelFromSupplier.ModelFromSupplierID});
        }

        // GET: ModelFromSuppliers/Delete/5
        [Authorize(Roles = "WarehouseManager, Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.ModelFromSupplier
                .Include(m => m.Model).Include(m=>m.StockReceiving)
                .SingleOrDefaultAsync(m => m.ModelFromSupplierID == id);
            if (item == null)
            {
                return NotFound();
            }

            var timeSpan = DateTime.Now - item.StockReceiving.Date;
            if (timeSpan.Hours > 2)
            {
                ViewData["ErrorText"] = "Không thể xóa sản phẩm sau 2 giờ";
                return View("ErrorPage");
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, item.StockReceiving,
                OrderOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            var listItems = await _context.Item.Where(i => i.ModelFromSupplierID == id).ToListAsync();
            ViewData["ModelItemID"] = item.ModelID;
            ViewData["ModelFromSupplierID"] = item.ModelFromSupplierID;
            ViewData["ModelID"] = new SelectList(_context.Model, "ModelID", "Name", item.ModelID);
            ViewData["StockReceivingID"] = new SelectList(_context.StockReceiving, "StockReceivingID", "StockReceivingID", item.StockReceivingID);
            var stockReceivingDetailVM = new StockReceivingDetailViewModel();
            stockReceivingDetailVM.ModelFromSupplier = item;
            stockReceivingDetailVM.Items = listItems;

            return View(stockReceivingDetailVM);
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
            return RedirectToAction("Details","StockReceivings",new {id = modelFromSupplier.StockReceivingID});
        }
        #region helper

        private bool ModelFromSupplierExists(int id)
        {
            return _context.ModelFromSupplier.Any(e => e.ModelFromSupplierID == id);
        }

        public async Task<ModelFromSupplier> ViewModelToModelFromSupplier(ModelFromSupplier modelFromSupplier)
        {
            var item = await _context.ModelFromSupplier.Include(m=>m.StockReceiving).SingleOrDefaultAsync(m =>
                m.ModelFromSupplierID == modelFromSupplier.ModelFromSupplierID);
            item.ModelFromSupplierID = modelFromSupplier.ModelFromSupplierID;
            item.ModelID = modelFromSupplier.ModelID;
            item.PriceBought = modelFromSupplier.PriceBought;
            item.PriceSold = modelFromSupplier.PriceSold;
            item.Quantity = modelFromSupplier.Quantity;
            item.Period = modelFromSupplier.Period;
            item.StockReceivingID = modelFromSupplier.StockReceivingID;
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
