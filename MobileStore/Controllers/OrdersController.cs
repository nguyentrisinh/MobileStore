using System;
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
            ViewData["CustomerSortParm"] = sortOrder=="customer" ? "customer_desc" : "customer";
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
                orders = orders.Include(m=>m.Customer).Include(m=>m.ApplicationUser).Where(s => s.Customer.Name.Contains(searchString));
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

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, order,
                OrderOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            var orderDetails = await _context.OrderDetail.Where(m => m.OrderID == id).Include(m=>m.Item).ToListAsync();
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

            // Fetch Contact from DB to get OwnerID. 
            var order = await _context.Order.SingleOrDefaultAsync(m => m.OrderID == id);
            if (order == null || id != sellViewModel.Order.OrderID)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, order,
                OrderOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }
            try

                {
                    _context.Update(sellViewModel.Order);
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
            
            return RedirectToAction(nameof(Edit), new {id });
           
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
            var newOrderDetails = await _context.OrderDetail.Where(m => m.OrderID == id).Select(m=>m.Item).Select(m=>new Item
            {
                IMEI = m.IMEI,
                ItemID = m.ItemID,
                ModelFromSupplierID = m.ModelFromSupplierID,
                ModelID = m.ModelID,
                Name = m.Name,

            }).ToListAsync();
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


        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderID == id);
        }
    }
}
