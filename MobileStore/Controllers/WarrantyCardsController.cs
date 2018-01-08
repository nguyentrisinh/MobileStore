using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using MobileStore.Authorization;
using MobileStore.Data;
using MobileStore.Models;
using MobileStore.Models.SellViewModel;
using MobileStore.Models.WarrantyCardViewModels;

namespace MobileStore.Controllers
{
    public class WarrantyCardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public WarrantyCardsController(ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;

        }
        [Authorize(Roles = "Technical,Sale,Admin")]
        public async Task<IActionResult> PrintWarrantyCard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var warrantyCard = await _context.WarrantyCard.Include(m=>m.Item).ThenInclude(m=>m.ModelFromSupplier).ThenInclude(m=>m.Model).ThenInclude(m=>m.Brand).SingleAsync(m => m.WarrantyCardID == id);
            if (warrantyCard == null)
            {
                return NotFound();
            }
            return View(warrantyCard);
        }
        [HttpPost, ActionName("PrintWarrantyCard")]
        [Authorize(Roles = "Technical,Sale,Admin")]
        public async Task<IActionResult> PrintWarrantyCardConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var warrantyCard = await _context.WarrantyCard.Include(m => m.Item).ThenInclude(m => m.Model).ThenInclude(m => m.Brand).SingleAsync(m => m.WarrantyCardID == id);
            if (warrantyCard == null)
            {
                return NotFound();
            }
            warrantyCard.IsPrinted = true;
            _context.Update(warrantyCard);
            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", new { id });
        }

        [Authorize(Roles = "Technical,Sale,Admin")]
        public async Task<IActionResult> PrintWarrantyDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var warrantyDetail = await _context.WarrantyDetail.Include(m=>m.WarrantyCard).ThenInclude(m=>m.Item).ThenInclude(m => m.ModelFromSupplier).ThenInclude(m=>m.Model).ThenInclude(m=>m.Brand).SingleAsync(m => m.WarrantyDetailID == id);
            if (warrantyDetail == null)
            {
                return NotFound();
            }
            return View(warrantyDetail);
        }
        [HttpPost,ActionName("PrintWarrantyDetail")]

        [Authorize(Roles = "Technical,Sale,Admin")]
        public async Task<IActionResult> PrintWarrantyDetailConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var warrantyDetail = await _context.WarrantyDetail.Include(m => m.WarrantyCard).ThenInclude(m => m.Item).ThenInclude(m => m.ModelFromSupplier).ThenInclude(m => m.Model).ThenInclude(m => m.Brand).SingleAsync(m => m.WarrantyDetailID == id);
            if (warrantyDetail == null)
            {
                return NotFound();
            }
            warrantyDetail.IsPrinted = true;
            _context.Update(warrantyDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", new { id = warrantyDetail.WarrantyCardID });
        }


        // GET: WarrantyCards
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["WCIDParm"] = String.IsNullOrEmpty(sortOrder) ? "WCI_desc" : "";
            ViewData["StartDateParm"] = sortOrder == "startdate" ? "startdate_desc" : "startdate";
            ViewData["EndDateParm"] = sortOrder == "enddate" ? "enddate_desc" : "enddate";
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

            var warrantyCards = from m in _context.WarrantyCard select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                warrantyCards = warrantyCards.Include(m => m.Item).ThenInclude(m=>m.Model).Include(m => m.ApplicationUser).Where(s => s.WarrantyCardID.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "WCIDParm":
                    warrantyCards = warrantyCards.OrderByDescending(s => s.WarrantyCardID).Include(m => m.Item).ThenInclude(m => m.Model).Include(m => m.ApplicationUser);
                    break;

                case "startdate":
                    warrantyCards = warrantyCards.OrderBy(s => s.StartDate).Include(m => m.Item).ThenInclude(m => m.Model).Include(m => m.ApplicationUser);
                    break;
                case "startdate_desc":
                    warrantyCards = warrantyCards.OrderByDescending(s => s.StartDate).Include(m => m.Item).ThenInclude(m => m.Model).Include(m => m.ApplicationUser);
                    break;
                case "enddate":
                    warrantyCards = warrantyCards.OrderBy(s => s.EndDate).Include(m => m.Item).ThenInclude(m => m.Model).Include(m => m.ApplicationUser);
                    break;

                case "enddate_desc":
                    warrantyCards = warrantyCards.OrderByDescending(s => s.EndDate).Include(m => m.Item).ThenInclude(m => m.Model).Include(m => m.ApplicationUser);
                    break;
                case "staff":
                    warrantyCards = warrantyCards.OrderBy(s => s.ApplicationUser.FirstName).Include(m => m.Item).ThenInclude(m => m.Model).Include(m => m.ApplicationUser);
                    break;
                case "staff_desc":
                    warrantyCards = warrantyCards.OrderByDescending(s => s.ApplicationUser.FirstName).Include(m => m.Item).ThenInclude(m => m.Model).Include(m => m.ApplicationUser);
                    break;
                default:
                    warrantyCards = warrantyCards.OrderBy(s => s.WarrantyCardID).Include(m => m.Item).ThenInclude(m => m.Model).Include(m => m.ApplicationUser);
                    break;
            }
            int pageSize = 12;

            return View(await PaginatedList<WarrantyCard>.CreateAsync(warrantyCards.AsNoTracking(), page ?? 1, pageSize));
        }


        [Authorize(Roles = "Technical, Admin")]
        public async Task<IActionResult> Fixed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var warrantyDetail =await _context.WarrantyDetail.SingleAsync(m => m.WarrantyDetailID == id);
            if (warrantyDetail == null)
            {
                return NotFound();
            }
            if (warrantyDetail.IsPrinted == false)
            {
                ViewData["ErrorText"] = "Chua in phieu hen";
                return View("ErrorPage");
            }
            warrantyDetail.WarrantyDate = DateTime.Now;
            warrantyDetail.Status = WarrantyDetailStatus.Fixed;
            _context.Update(warrantyDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), "WarrantyCards", new { id = warrantyDetail.WarrantyCardID });
        }


        [Authorize(Roles = "Technical, Admin")]

        public async Task<IActionResult> Returned(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var warrantyDetail = await _context.WarrantyDetail.SingleAsync(m => m.WarrantyDetailID == id);
            if (warrantyDetail == null)
            {
                return NotFound();
            }
            if (warrantyDetail.IsPrinted == false)
            {
                ViewData["ErrorText"] = "Chua in phieu hen";
                return View("ErrorPage");
            }
            warrantyDetail.ReturnedDate = DateTime.Now;
            warrantyDetail.Status = WarrantyDetailStatus.Returned;
            _context.Update(warrantyDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), "WarrantyCards", new { id = warrantyDetail.WarrantyCardID });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles= "Technical, Admin")]
        public async Task<IActionResult> CreateReturnItem(WarrantyCardViewModel warrantyCardVm)
        {
            var warrantyCard = await _context.WarrantyCard.Include(m => m.Item).ThenInclude(m => m.Model).SingleOrDefaultAsync(m => m.ItemID == warrantyCardVm.ReturnItem.OldItemID);
            if (ModelState.IsValid)
            {
               
                var returnDeadline = await _context.Constant.SingleAsync(m => m.ConstantID == 1);
                var anyReturnItem = await _context.ReturnItem.Where(m => m.OldItemID == warrantyCard.ItemID).AnyAsync();
                if (DateTime.Now <= warrantyCard.StartDate.AddDays(returnDeadline.Parameter) && !anyReturnItem &&
                    warrantyCard.IsPrinted)
                {
                    // Tạo thông tin return item
                    var returnItem = new ReturnItem();
                    returnItem.OldItemID = warrantyCardVm.ReturnItem.OldItemID;
                    returnItem.DefectInfo = warrantyCardVm.ReturnItem.DefectInfo;
                    returnItem.NewItemID = warrantyCardVm.ReturnItem.NewItemID;
                    returnItem.ReturnDate = DateTime.Now;
                    returnItem.ApplicationUserID = _userManager.GetUserId(User);

                    // Tạo warrantycard cho sản phẩm đổi

                    var newWarrantyCard = new WarrantyCard();
                    newWarrantyCard.NumberOfWarranty = 0;
                    newWarrantyCard.StartDate = DateTime.Now;
                    var itemInfo = await _context.Item.Where(m => m.ItemID == warrantyCardVm.ReturnItem.NewItemID)
                        .Include(m => m.ModelFromSupplier).SingleOrDefaultAsync();
                    newWarrantyCard.EndDate = DateTime.Now.AddMonths(itemInfo.ModelFromSupplier.Period);
                    newWarrantyCard.IsPrinted = false;
                    newWarrantyCard.IsDisabled = false;
                    newWarrantyCard.ItemID = warrantyCardVm.ReturnItem.NewItemID;
                    newWarrantyCard.ApplicationUserID = _userManager.GetUserId(User);

                    // Disable warrantyCard cũ
                    warrantyCard.IsDisabled = true;

                    // Cap nhat tinh trang san pham cu
                    var oldItem = await _context.Item.SingleAsync(m => m.ItemID == warrantyCardVm.ReturnItem.OldItemID);
                    oldItem.Status = ItemStatus.Returned;

                    // Cap nhat tinh trang san pham moi

                    var newItem = await _context.Item.SingleAsync(m => m.ItemID == warrantyCardVm.ReturnItem.NewItemID);
                    newItem.Status = ItemStatus.Sold;

                    _context.UpdateRange(oldItem, newItem);
                    _context.Add(newWarrantyCard);
                    _context.Update(warrantyCard);
                    _context.Add(returnItem);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ViewData["ErrorText"] = "Khong the Doi tra";
                    return View("ErrorPage");
                }
            }
            return RedirectToAction(nameof(Detail),"WarrantyCards",new {id=warrantyCard.WarrantyCardID});
        }




        [HttpPost]

        [Authorize(Roles = "Technical, Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWarrantyDetail(WarrantyCardViewModel warrantyCardVm)
        {
            var warrantyCard = await _context.WarrantyCard.Include(m => m.Item).ThenInclude(m => m.Model).SingleOrDefaultAsync(m => m.WarrantyCardID==warrantyCardVm.WarrantyDetail.WarrantyCardID);
            if (warrantyCard == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {

                var returnDeadline = await _context.Constant.SingleAsync(m => m.ConstantID == 1);
                var anyReturnItem = await _context.ReturnItem.Where(m => m.OldItemID == warrantyCard.ItemID).AnyAsync();
                var anyWatingWarrant = await _context.WarrantyDetail.Where(m => (
                    (m.WarrantyCardID == warrantyCard.WarrantyCardID) && (
                        m.Status == WarrantyDetailStatus.Fixing || m.Status == WarrantyDetailStatus.Fixed))).AnyAsync();
                if (warrantyCard.CanWarrant() && !anyReturnItem && !anyWatingWarrant && warrantyCard.IsPrinted)
                {
                    var warrantyDetail  = new WarrantyDetail();
                    warrantyDetail.Date = DateTime.Now;
                    warrantyDetail.DefectInfo = warrantyCardVm.WarrantyDetail.DefectInfo;
                    warrantyDetail.ExpectedDate = warrantyCardVm.WarrantyDetail.ExpectedDate;
                    warrantyDetail.ReturnedDate = null;
                    warrantyDetail.Status = WarrantyDetailStatus.Fixing;
                    warrantyDetail.WarrantyCardID = warrantyCardVm.WarrantyDetail.WarrantyCardID;
                    warrantyDetail.WarrantyDate = null;
                    warrantyDetail.IsPrinted = false;
                    warrantyDetail.ApplicationUserID = _userManager.GetUserId(User);
                    warrantyCard.NumberOfWarranty += 1;
                    _context.Update(warrantyCard);
                    _context.Update(warrantyDetail);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ViewData["ErrorText"] = "Khong the them chi tiet bao hanh";
                    return View("ErrorPage");
                }
            }
            return RedirectToAction(nameof(Detail), "WarrantyCards", new { id = warrantyCardVm.WarrantyDetail.WarrantyCardID });
        }


        // GET: WarrantyCards/Details/5
        #region Get Edit
        // GET: Orders/Edit/5
        public async Task<IActionResult> Detail(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }


            var warrantyCard = await _context.WarrantyCard.Include(m=>m.Item).ThenInclude(m=>m.Model).SingleOrDefaultAsync(m => m.WarrantyCardID == id);


            if (warrantyCard == null)
            {
                return NotFound();
            }
            // NGƯỜI KHÁC CÓ THỂ XEM BẢO HẢNH KHÔNG

            //var isAuthorized = await _authorizationService.AuthorizeAsync(User, warrantyCard,
            //    OrderOperations.Update);
            //if (!isAuthorized.Succeeded)
            //{
            //    return new ChallengeResult();
            //}
            var warrantyCardVm = new WarrantyCardViewModel();
            warrantyCardVm.WarrantyCard = warrantyCard;

            

            var returnDeadline = await _context.Constant.SingleAsync(m => m.ConstantID == 1);
            // Kiểm tra xem sản phẩm này đã bị đổi chưa
            var isReturnItem = await _context.ReturnItem.Where(m => m.OldItemID == warrantyCard.ItemID).AnyAsync();
            // Nếu đã bị đổi thì gửi thông tin đổi qua view
            if (isReturnItem)
            {
                var returnItem = await _context.ReturnItem.Include(m => m.OldItem).Include(m => m.NewItem)
                    .SingleAsync(m => m.OldItemID == warrantyCard.ItemID);
                var newWarrantyCard = await _context.WarrantyCard.SingleAsync(m => m.ItemID == returnItem.NewItemID);
                ViewData["WarrantyCardID"] = newWarrantyCard.WarrantyCardID;
                warrantyCardVm.ReturnItem = returnItem;
            }
            // Xem warrantyCarrd này còn có thể đổi trả ko?
            warrantyCardVm.CanReturn = DateTime.Now <= warrantyCard.StartDate.AddDays(returnDeadline.Parameter) && !isReturnItem ;

            //Return Item, WarrantyDetail dung de nhan Post
            var warrantyDetails =await _context.WarrantyDetail.Where(m => m.WarrantyCardID == id).OrderByDescending(m=>m.Date).ToListAsync();
            warrantyCardVm.WarrantyDetails = warrantyDetails;
            

            var items = await _context.Item
                .Where(m => m.ModelID == warrantyCard.Item.ModelID && m.Status == ItemStatus.InStock).ToListAsync();
            warrantyCardVm.Items = items;
            if (isReturnItem)
            {
                warrantyCardVm.WarrantyCardStatus = WarrantyCardStatus.Returned;
            }
            else
            {
                
                if (await _context.WarrantyDetail.Where(m =>(
                    (m.WarrantyCardID == warrantyCard.WarrantyCardID) && (
                        m.Status == WarrantyDetailStatus.Fixing || m.Status == WarrantyDetailStatus.Fixed))).AnyAsync())
                {
                    warrantyCardVm.WarrantyCardStatus = WarrantyCardStatus.Waiting;
                }
                else
                {
                    if (warrantyCard.CanWarrant())
                    {
                        warrantyCardVm.WarrantyCardStatus = WarrantyCardStatus.CanWarrant;
                    }
                    else
                    {
                        warrantyCardVm.WarrantyCardStatus = WarrantyCardStatus.Expired;
                    }

                }
            }
            return View(warrantyCardVm);
        }
        #endregion

        private bool WarrantyCardExists(int id)
        {
            return _context.WarrantyCard.Any(e => e.WarrantyCardID == id);
        }
    }
}
