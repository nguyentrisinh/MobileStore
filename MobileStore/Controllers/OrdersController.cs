using System;
using System.Collections.Generic;
using MobileStore.Authorization;
using MobileStore.Data;
using MobileStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MobileStore.Models.SellViewModel;

namespace MobileStore.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;

        }
        #region Index
        // GET: Orders
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["CustomerSortParm"] = sortOrder == "customer" ? "customer_desc" : "customer";
            ViewData["TotalSortParm"] = sortOrder == "total" ? "total_desc" : "total";
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

            var orders = from m in _context.Order select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Include(m => m.Customer).Include(m => m.ApplicationUser).Where(s => s.Customer.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "customer_desc":
                    orders = orders.OrderByDescending(s => s.Customer.Name).Include(m => m.Customer).Include(m => m.ApplicationUser);
                    break;
                case "customer":
                    orders = orders.OrderBy(s => s.Customer).Include(m => m.Customer).Include(m => m.ApplicationUser);
                    break;
                case "date_desc":
                    orders = orders.OrderByDescending(s => s.Date).Include(m => m.Customer).Include(m => m.ApplicationUser);
                    break;
                case "total":
                    orders = orders.OrderBy(s => s.Total).Include(m => m.Customer).Include(m => m.ApplicationUser);
                    break;
                case "total_desc":
                    orders = orders.OrderByDescending(s => s.Total).Include(m => m.Customer).Include(m => m.ApplicationUser);
                    break;
                case "staff":
                    orders = orders.OrderBy(s => s.ApplicationUser.FirstName).Include(m => m.Customer).Include(m => m.ApplicationUser);
                    break;
                case "staff_desc":
                    orders = orders.OrderByDescending(s => s.ApplicationUser.FirstName).Include(m => m.Customer).Include(m => m.ApplicationUser);
                    break;
                default:
                    orders = orders.OrderBy(s => s.Date).Include(m => m.Customer).Include(m => m.ApplicationUser);
                    break;
            }
            int pageSize = 1;

            return View(await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(), page ?? 1, pageSize));
        }
        #endregion
        #region Detail

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.ApplicationUser)
                .Include(o => o.Customer)
                .SingleOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        #endregion

        #region Get Create

        // GET: Orders/Create
        [Authorize(Roles = "Sale,Admin")]
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customer, "CustomerID", "Name");
            return View();
        }
        #endregion
        #region Post Create

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,Total,Date,CustomerID")] Order order)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CustomerID"] = new SelectList(_context.Customer, "CustomerID", "Name", order.CustomerID);
                return View(order);
                //var userID = User.Identity.
            }
            _context.Add(order);
            order.ApplicationUserID = _userManager.GetUserId(User);
            order.Date = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Get Edit
        // GET: Orders/Edit/5
        [Authorize(Roles = "Sale,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            
            var order = await _context.Order.SingleOrDefaultAsync(m => m.OrderID == id);


            if (order == null)
            {
                return NotFound();
            }

            //var timeSpan = DateTime.Now - order.Date;
            //if (timeSpan.Hours > 2)
            //{
            //    ViewData["ErrorText"] = "Bạn không thể sửa sau 2h";
            //    return View("ErrorPage");
            //}


            var isAuthorized = await _authorizationService.AuthorizeAsync(User, order,
                OrderOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            var orderDetails = await _context.OrderDetail.Where(m => m.OrderID == id).Include(m => m.Item).ToListAsync();
            var sellViewModel = new SellViewModel();
            sellViewModel.Order = order;
            sellViewModel.OrderDetails = orderDetails;
            sellViewModel.Customers = _context.Customer;
            sellViewModel.NewItems = _context.Item.Where(m => m.Status == ItemStatus.New).Select(m => new
            {
                m.ItemID,
                DisplayName = m.Name + m.IMEI
            });
            return View(sellViewModel);
        }
        #endregion

        #region Post Edit



        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Sale,Admin")]
        public async Task<IActionResult> Edit(int id, SellViewModel sellViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(sellViewModel);
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, sellViewModel.Order,
                OrderOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            try
            {
                var newOrder = ViewModelToOrder(sellViewModel).Result;
                var timeSpan = DateTime.Now - newOrder.Date;
                if (timeSpan.Hours > 2)
                {
                    ViewData["ErrorText"] = "Bạn không thể sửa sau 2h";
                    return View("ErrorPage");
                }

                _context.Update(newOrder);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(sellViewModel.Order.OrderID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Edit), new { id });

        }
        #endregion

        #region Get Delete
        // GET: Orders/Delete/5
        [Authorize(Roles = "Sale,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.ApplicationUser)
                .Include(o => o.Customer)
                .SingleOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            var timeSpan = DateTime.Now - order.Date;
            if (timeSpan.Hours >= 2)
            {
                ViewData["ErrorText"] = "Bạn không thể xóa sau 2h";
                return View("ErrorPage");
            }

            var hasDetails = await _context.OrderDetail.Where(m => m.OrderID == id).AnyAsync();
            if (hasDetails)
            {
                ViewData["ErrorText"] = "Không thể xóa do đã có sản phẩm bán ra ";
                return View("ErrorPage");
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, order,
                OrderOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }



            return View(order);
        }

        #endregion

        #region Post Delete
        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Sale,Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var order = await _context.Order.SingleOrDefaultAsync(m => m.OrderID == id);

            var timeSpan = DateTime.Now - order.Date;
            if (timeSpan.Hours > 2)
            {
                ViewData["ErrorText"] = "Bạn không thể xóa sau 2h";
                return View("ErrorPage");
            }
            var hasDetails = await _context.OrderDetail.Where(m => m.OrderID == id).AnyAsync();
            if (hasDetails)
            {
                ViewData["ErrorText"] = "Không thể xóa do đã có sản phẩm bán ra ";
                return View("ErrorPage");
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, order,
                OrderOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion


        #region Post CreateOrderDetail
        // POST: OrderDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Sale,Admin")]
        public async Task<IActionResult> CreateOrderDetail(SellViewModel sellViewModel)
        {
            if (ModelState.IsValid)
            {
                var order = await _context.Order.SingleAsync(m => m.OrderID == sellViewModel.OrderDetail.OrderID);
                var timeSpan = DateTime.Now - order.Date;
                if (timeSpan.Hours > 2)
                {
                    ViewData["ErrorText"] = "Bạn không thể sửa sau 2h";
                    return View("ErrorPage");
                }
                _context.Add(sellViewModel.OrderDetail);
                var itemID = sellViewModel.OrderDetail.ItemID;
                var item = await _context.Item.SingleAsync(m => m.ItemID == itemID);
                item.Status = ItemStatus.Sold;
                _context.Update(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), new { id = sellViewModel.OrderDetail.OrderID });
        }
        #endregion

        #region GET Delete OrderDetail

        // GET: OrderDetails/Delete/5
        [Authorize(Roles = "Sale,Admin")]
        public async Task<IActionResult> DeleteOrderDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
           
            var orderDetail = await _context.OrderDetail
                .Include(o => o.Item)
                .Include(o => o.Order)
                .SingleOrDefaultAsync(m => m.OrderDetailID == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            
            var timeSpan = DateTime.Now - orderDetail.Order.Date;
            if (timeSpan.Hours > 2)
            {
                ViewData["ErrorText"] = "Không thể xóa sản phẩm sau 2 giờ";
                return View("ErrorPage");
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, orderDetail.Order,
                OrderOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            return View(orderDetail);
        }
        #endregion

        #region POST Delete OrderDetail

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("DeleteOrderDetail")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Sale,Admin")]
        public async Task<IActionResult> DeleteOrderDetailConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetail.Include(m=>m.Order).Include(m=>m.Item).SingleOrDefaultAsync(m => m.OrderDetailID == id);
            
            var timeSpan = DateTime.Now - orderDetail.Order.Date;
            if (timeSpan.Hours > 2)
            {
                ViewData["ErrorText"] = "Không thể xóa sản phẩm sau 2 giờ";
                return View("ErrorPage");
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, orderDetail.Order,
                OrderOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            orderDetail.Item.Status = ItemStatus.New;
            _context.Update(orderDetail.Item);
            _context.OrderDetail.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit),new {id=orderDetail.OrderID});
        }

        #endregion


        #region GET Edit OrderDetail 
        [Authorize(Roles = "Sale,Admin")]
        public async Task<IActionResult> EditOrderDetail(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetail.Include(m=>m.Order).SingleOrDefaultAsync(m => m.OrderDetailID == id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            var timeSpan = DateTime.Now - orderDetail.Order.Date;
            if (timeSpan.Hours > 2)
            {
                ViewData["ErrorText"] = "Không thể xóa sản phẩm sau 2 giờ";
                return View("ErrorPage");
            }



            var isAuthorized = await _authorizationService.AuthorizeAsync(User, orderDetail.Order,
                OrderOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }


            ViewData["ItemID"] = new SelectList(_context.Item, "ItemID", "ItemID", orderDetail.ItemID);
            ViewData["OrderID"] = new SelectList(_context.Order, "OrderID", "OrderID", orderDetail.OrderID);
            return View(orderDetail);
        }


        #endregion

        #region POST Edit OrderDetail

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Sale,Admin")]
        public async Task<IActionResult> EditOrderDetail(int id, [Bind("OrderDetailID,PriceSold,ItemID,OrderID")] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailID)
            {
                return NotFound();
            }
            var order = await _context.Order.SingleOrDefaultAsync(m => m.OrderID == orderDetail.OrderID);
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, order,
                OrderOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var newOrderDetail = ViewModelToModelOrderDetail(orderDetail).Result;
                    var timeSpan = DateTime.Now - newOrderDetail.Order.Date;
                    if (timeSpan.Hours > 2)
                    {
                        ViewData["ErrorText"] = "Không thể xóa sản phẩm sau 2 giờ";
                        return View("ErrorPage");
                    }
                    _context.Update(newOrderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.OrderDetailID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["ItemID"] = new SelectList(_context.Item, "ItemID", "ItemID", orderDetail.ItemID);
            ViewData["OrderID"] = new SelectList(_context.Order, "OrderID", "OrderID", orderDetail.OrderID);
            return RedirectToAction(nameof(Edit), new { id = orderDetail.OrderID });
        }

        #endregion

        #region helper
        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetail.Any(e => e.OrderDetailID == id);
        }
        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderID == id);
        }

        public async Task<Order> ViewModelToOrder(SellViewModel sellViewModel)
        {
            var item = await _context.Order.SingleOrDefaultAsync(m =>
                m.OrderID == sellViewModel.Order.OrderID);
            item.OrderID = sellViewModel.Order.OrderID;
            item.CustomerID = sellViewModel.Order.CustomerID;
            item.Total = sellViewModel.Order.Total;
            return item;
        }

        public async Task<OrderDetail> ViewModelToModelOrderDetail(OrderDetail orderDetail)
        {
            var item = await _context.OrderDetail.Include(m=>m.Order).SingleOrDefaultAsync(m =>
                m.OrderDetailID == orderDetail.OrderDetailID);
            item.OrderID = orderDetail.OrderID;
            item.OrderDetailID = orderDetail.OrderDetailID;
            item.ItemID = orderDetail.ItemID;
            item.PriceSold = orderDetail.PriceSold;
            return item;
        }
        #endregion
    }
}
