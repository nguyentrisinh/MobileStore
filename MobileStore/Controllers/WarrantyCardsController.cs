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

        // GET: WarrantyCards
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.WarrantyCard.Include(w => w.Item);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: WarrantyCards/Details/5
        #region Get Edit
        // GET: Orders/Edit/5
        [Authorize(Roles = "Sale,Admin")]
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
            var returnItem = await _context.ReturnItem.Where(m => m.OldItemID == warrantyCard.ItemID).AnyAsync();
            // Nếu đã bị đổi thì gửi thông tin đổi qua view
            if (returnItem)
            {
               
                warrantyCardVm.ReturnItem =
                    await _context.ReturnItem.SingleAsync(m => m.OldItemID == warrantyCard.ItemID);
            }
            // Xem warrantyCarrd này còn có thể đổi trả ko?
            warrantyCardVm.CanReturn = DateTime.Now <= warrantyCard.StartDate.AddDays(returnDeadline.Parameter) && !returnItem ;

            //Return Item, WarrantyDetail dung de nhan Post
            var warrantyDetails =await _context.WarrantyDetail.Where(m => m.WarrantyCardID == id).ToListAsync();
            warrantyCardVm.WarrantyDetails = warrantyDetails;
            

            var items = await _context.Item
                .Where(m => m.ModelID == warrantyCard.Item.ModelID && m.Status == ItemStatus.InStock).ToListAsync();
            warrantyCardVm.Items = items;

            warrantyCardVm.CanWarrant = warrantyCard.CanWarrant() && !await _context.WarrantyDetail.Where(m =>
                                            m.WarrantyCardID == warrantyCard.WarrantyCardID &&(
                                            m.Status == WarrantyDetailStatus.Fixing || m.Status == WarrantyDetailStatus.Fixed)).AnyAsync();

            return View(warrantyCardVm);
        }
        #endregion

        //        #region Post Edit



        //        // POST: Orders/Edit/5
        //        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        [Authorize(Roles = "Sale,Admin")]
        //        public async Task<IActionResult> Detail(int id, SellViewModel sellViewModel)
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                return View(sellViewModel);
        //            }

        //            var isAuthorized = await _authorizationService.AuthorizeAsync(User, sellViewModel.Order,
        //                OrderOperations.Update);
        //            if (!isAuthorized.Succeeded)
        //            {
        //                return new ChallengeResult();
        //            }
        //            try
        //            {
        //                var newOrder = ViewModelToOrder(sellViewModel).Result;
        //                var timeSpan = DateTime.Now - newOrder.Date;
        //                if (timeSpan.Hours > 2)
        //                {
        //                    ViewData["ErrorText"] = "Bạn không thể sửa sau 2h";
        //                    return View("ErrorPage");
        //                }

        //                _context.Update(newOrder);
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!OrderExists(sellViewModel.Order.OrderID))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }

        //            return RedirectToAction(nameof(Edit<>), new { id });

        //        }
        //#endregion

        private bool WarrantyCardExists(int id)
        {
            return _context.WarrantyCard.Any(e => e.WarrantyCardID == id);
        }
    }
}
