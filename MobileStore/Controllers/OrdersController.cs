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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MobileStore.Models.OrderDetailViewModels;
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
        public async Task<IActionResult> Print(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var order = await _context.Order.Include(m=>m.Customer).Include(m=>m.OrderDetails).ThenInclude(m=>m.Item).ThenInclude(m=>m.Model).SingleAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }


            return View(order);
        }

        [HttpPost, ActionName("Print")]
        [Authorize(Roles = "Sales,Admin")]
        // GET: Orders
        public async Task<IActionResult> PrintConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var order =await _context.Order.SingleAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }
            order.IsPrinted = true;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Detail", new {id});
        }
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
            int pageSize = 12;

            return View(await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(), page ?? 1, pageSize));
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
            order.Total = 0;
            order.ApplicationUserID = _userManager.GetUserId(User);
            order.Date = DateTime.Now;
            _context.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail),new {id=order.OrderID});
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
            // Kiem tra da in hay chua

            //if (order.IsPrinted)
            //{
            //    ViewData["ErrorText"] = "Bạn không thể sửa sau khi in";
            //    return View("ErrorPage");
            //}
            // Kiem tra quyen

            // Cho xem edit

            //var isAuthorized = await _authorizationService.AuthorizeAsync(User, order,
            //    OrderOperations.Update);
            //if (!isAuthorized.Succeeded)
            //{
            //    return new ChallengeResult();
            //}

            // Tien hanh thuc hien
            
           
            
            ViewData["Customers"] =new SelectList(_context.Customer,"CustomerID","Name",order.CustomerID);
            return View(order);
        }
        #endregion


        #region Detail
        // GET: Orders/Edit/5
        [Authorize(Roles = "Sale,Admin")]
        public async Task<IActionResult> Detail(int? id, string sortOrder, string currentFilter, string searchString, int? page)
        {
            #region Check Exists
            if (id == null)
            {
                return NotFound();
            }


            var order = await _context.Order.Include(m=>m.Customer).SingleOrDefaultAsync(m => m.OrderID == id);


            if (order == null)
            {
                return NotFound();
            }

            #endregion

            #region Filter and Search
            ViewData["ItemModelNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "item_model_name_desc" : "";
            ViewData["ItemNameSortParm"] = sortOrder == "item_name" ? "item_name_desc" : "item_name";
            ViewData["IMEISortParm"] = sortOrder == "imei" ? "imei_desc" : "imei";
            ViewData["SerializerNumberSortParm"] = sortOrder == "serializer_number" ? "serializer_number_desc" : "serializer_number";
            ViewData["PriceSoldSortParm"] = sortOrder == "pricesold" ? "pricesold_desc" : "pricesold";

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

            var orderDetails = _context.OrderDetail.Where(m => m.OrderID == id);

            if (!String.IsNullOrEmpty(searchString))
            {
                orderDetails = orderDetails.Include(m => m.Item).ThenInclude(m=>m.Model).Where(m => m.Item.Model.Name.ToLower().Contains(searchString.ToLower())
                || m.Item.IMEI.ToString().ToLower().Contains(searchString.ToLower())
                || m.Item.SerializerNumber.ToString().ToLower().Contains(searchString.ToLower())
                );
            }
            switch (sortOrder)
            {
                case "item_model_name_desc":
                    orderDetails = orderDetails.Include(m => m.Item).ThenInclude(m => m.Model).OrderByDescending(s => s.Item.Model.Name);
                    break;
                case "item_name":
                    orderDetails = orderDetails.Include(m => m.Item).ThenInclude(m => m.Model).OrderBy(s => s.Item.Name);
                    break;
                case "item_name_desc":
                    orderDetails = orderDetails.Include(m => m.Item).ThenInclude(m => m.Model).OrderByDescending(s => s.Item.Name);
                    break;
                case "imei":
                    orderDetails = orderDetails.Include(m => m.Item).ThenInclude(m => m.Model).OrderBy(s => s.Item.IMEI);
                    break;
                case "imei_desc":
                    orderDetails = orderDetails.Include(m => m.Item).ThenInclude(m => m.Model).OrderByDescending(s => s.Item.IMEI);
                    break;

                case "serializer_number":
                    orderDetails = orderDetails.Include(m => m.Item).ThenInclude(m => m.Model).OrderBy(s => s.Item.SerializerNumber);
                    break;
                case "serializer_number_desc":
                    orderDetails = orderDetails.Include(m => m.Item).ThenInclude(m => m.Model).OrderByDescending(s => s.Item.SerializerNumber);
                    break;
                case "pricesold":
                    orderDetails = orderDetails.Include(m => m.Item).ThenInclude(m => m.Model).OrderBy(s => s.PriceSold);
                    break;
                case "pricesold_desc":
                    orderDetails = orderDetails.Include(m => m.Item).ThenInclude(m => m.Model).OrderByDescending(s => s.PriceSold);
                    break;
                default:
                    orderDetails = orderDetails.Include(m => m.Item).ThenInclude(m => m.Model).OrderBy(s => s.Item.Model.Name);
                    break;
            }
            #endregion

            
            var sellViewModel = new SellViewModel();
            sellViewModel.Order = order;
            sellViewModel.Customers = _context.Customer;
            sellViewModel.NewItems = _context.Item.Where(m => m.Status == ItemStatus.InStock).Include(m=>m.Model).Include(m=>m.ModelFromSupplier);
            sellViewModel.Models = _context.Model;
            #region Paging
            int pageSize = 12;
            PaginatedList<OrderDetail> pagesOrderDetails = await PaginatedList<OrderDetail>.CreateAsync(orderDetails.AsNoTracking(), page ?? 1, pageSize);
            sellViewModel.OrderDetails = pagesOrderDetails;
            #endregion
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
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, order,
                OrderOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            try
            {
                var newOrder = ViewModelToOrder(order).Result;
               // Kiem tra in thi k cho
                if (newOrder.IsPrinted)
                {
                    ViewData["ErrorText"] = "Bạn không thể sửa sau khi in";
                    return View("ErrorPage");
                }

                _context.Update(newOrder);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.OrderID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Detail), new { id });

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
            if (order.IsPrinted)
            {
                ViewData["ErrorText"] = "Bạn không thể xóa sau khi in";
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
            if (order.IsPrinted)
            {
                ViewData["ErrorText"] = "Bạn không thể xóa sau khi in";
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

                // Check da in hay chua
                if (order.IsPrinted == true)
                {
                    ViewData["ErrorText"] = "Bạn không thể sửa sau khi in";
                    return View("ErrorPage");
                }

                //Kiem tra co quyen them ko
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, order,
                    OrderOperations.Update);
                if (!isAuthorized.Succeeded)
                {
                    return new ChallengeResult();
                }

                var warrantyCard = new WarrantyCard();
                warrantyCard.NumberOfWarranty = 0;
                warrantyCard.StartDate = DateTime.Now;
                var itemInfo = await _context.Item.Where(m=>m.ItemID==sellViewModel.OrderDetail.ItemID).Include(m=>m.ModelFromSupplier).SingleOrDefaultAsync();
                warrantyCard.EndDate= DateTime.Now.AddMonths(itemInfo.ModelFromSupplier.Period);
                warrantyCard.IsPrinted = false;
                warrantyCard.IsDisabled = false;
                warrantyCard.ItemID = sellViewModel.OrderDetail.ItemID;
                warrantyCard.ApplicationUserID = _userManager.GetUserId(User);
                _context.Add(warrantyCard);


                _context.Add(sellViewModel.OrderDetail);


                var itemID = sellViewModel.OrderDetail.ItemID;
                var item = await _context.Item.SingleAsync(m => m.ItemID == itemID);
                item.Status = ItemStatus.Sold;
                _context.Update(item);

                order.Total += sellViewModel.OrderDetail.PriceSold;
                _context.Update(order);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Detail), new { id = sellViewModel.OrderDetail.OrderID });
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



            if (orderDetail.Order.IsPrinted == true)
            {
                ViewData["ErrorText"] = "Không thể xóa sản phẩm sau khi in";
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
           // Kiem tra in thi ko duoc xoa
            if (orderDetail.Order.IsPrinted==true)
            {
                ViewData["ErrorText"] = "Không thể xóa sản phẩm sau khi in";
                return View("ErrorPage");
            }

            // Kiem tra quyen co phai owner ko, neu la owner thi moi dc xoa
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, orderDetail.Order,
                OrderOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            try
            {
                // Cap nhan tinh trang san pham la da chua ban
                orderDetail.Item.Status = ItemStatus.InStock;
                _context.Update(orderDetail.Item);

                // Xoa WarrantyCard
                var warrantyCard = await _context.WarrantyCard.SingleAsync(m => m.ItemID == orderDetail.ItemID);
                _context.WarrantyCard.Remove(warrantyCard);
                _context.OrderDetail.Remove(orderDetail);
                // Cap nhat total cua Order

                orderDetail.Order.Total -= orderDetail.PriceSold;
                _context.Update(orderDetail.Order);

                await _context.SaveChangesAsync();
            }
            catch
            {

                throw;
            }
           
            return RedirectToAction(nameof(Detail),new {id=orderDetail.OrderID});
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
            // Code Khóa sửa tại đây check đã in
           
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, orderDetail.Order,
                OrderOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            
            ViewData["InStockItems"] = new SelectList(_context.Item.Where(m => (m.Status == ItemStatus.InStock) || (m.ItemID == orderDetail.ItemID)),"ItemID","Name",orderDetail.ItemID);

           
            return View(orderDetail);
        }


        #endregion



        #region POST Edit OrderDetail

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Sale,Admin")]
        public async Task<IActionResult> EditOrderDetail(int id, OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailID)
            {
                return NotFound();
            }
            var order = await _context.Order.SingleOrDefaultAsync(m => m.OrderID == orderDetail.OrderID);
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, order,
                OrderOperations.Update);
            if (order.IsPrinted)
            {
                ViewData["ErrorText"] = "Không thể sửa sau khi in";
                return View("ErrorPage");
            }
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var oldOrderDetail = await _context.OrderDetail.SingleAsync(m => m.OrderDetailID == id);
                    order.Total += orderDetail.PriceSold - oldOrderDetail.PriceSold;

                    var newWarrantyCard = await _context.WarrantyCard.SingleAsync(m => m.ItemID == oldOrderDetail.ItemID);
                    newWarrantyCard.ItemID = orderDetail.ItemID;
                    var oldItem = await _context.Item.SingleAsync(m => m.ItemID == oldOrderDetail.ItemID);
                    oldItem.Status = ItemStatus.InStock;

                    var newOrderDetail = ViewModelToModelOrderDetail(orderDetail).Result;

                    var newItem = await _context.Item.SingleAsync(m => m.ItemID == newOrderDetail.ItemID);
                    newItem.Status = ItemStatus.Sold;

                   
                    _context.Update(order);



                    _context.Item.UpdateRange(oldItem,newItem);

                    _context.Update(newWarrantyCard);
                    // Code check khóa tại đây kiểm tra print
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
            return RedirectToAction(nameof(Detail), new { id = orderDetail.OrderID });
        }

        #endregion


        #region GET Detail OrderDetail 
        [Authorize(Roles = "Sale,Admin")]
        public async Task<IActionResult> DetailOrderDetail(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetail.Include(m => m.Order).Include(m=>m.Item).ThenInclude(m=>m.Model).SingleOrDefaultAsync(m => m.OrderDetailID == id);

            if (orderDetail == null)
            {
                return NotFound();
            }
            // Code Khóa sửa tại đây check đã in

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, orderDetail.Order,
                OrderOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }


            var editOrderDetailVM = new EditOrderDetailViewModel();
            editOrderDetailVM.OrderDetail = orderDetail;

            var warrantyCard = await _context.WarrantyCard.Include(m => m.Item).ThenInclude(m => m.Model).SingleOrDefaultAsync(m => m.ItemID == orderDetail.ItemID);
            editOrderDetailVM.WarrantyCard = warrantyCard;
            var items = await _context.Item.Where(m => (m.Status == ItemStatus.InStock) || (m.ItemID == orderDetail.ItemID)).ToListAsync();
            editOrderDetailVM.Items = items;
            var orders = await _context.Order.Where(m => (m.IsPrinted == false) || (m.OrderID == orderDetail.OrderID)).ToListAsync();
            editOrderDetailVM.Orders = orders;

            var returnDeadline = await _context.Constant.Where(m => m.ConstantID == 1).SingleAsync();
            if (DateTime.Now < warrantyCard.StartDate.AddDays(returnDeadline.Parameter))
            {
                editOrderDetailVM.CanReturn = true;
            }
            else
            {
                editOrderDetailVM.CanReturn = false;
            }
            return View(editOrderDetailVM);
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

        public async Task<Order> ViewModelToOrder(Order order)
        {
            var item = await _context.Order.SingleOrDefaultAsync(m =>
                m.OrderID == order.OrderID);
            item.CustomerID = order.CustomerID;
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
