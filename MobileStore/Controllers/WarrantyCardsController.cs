using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MobileStore.Data;
using MobileStore.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace MobileStore.Controllers
{
    public class WarrantyCardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        [HttpPost]
        public  async Task< IActionResult> StorageLocations()
        {


            var k = _context.Item.ToList();
            List<String> kl = new List<String>();
            for(int i=0; i < k.Count();i++)
            {
                kl.Add(k[i].IMEI.ToString());
            }
            var json = JsonConvert.SerializeObject( kl );
            return Json( json);
        }
        public WarrantyCardsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: WarrantyCards
        public async Task<IActionResult> Index(string SearchString, string sortOrder, string currentFilter, int? page)
        {
            ViewData["DateStartSortParm"] = sortOrder == "DateStart" ? "datestart_desc" : "DateStart";
            ViewData["DateEndSortParm"] = sortOrder == "DateEnd" ? "dateend_desc" : "DateEnd";
            ViewData["CurrentSort"] = sortOrder;

            if (SearchString !=null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }

            ViewData["CurrentFilter"] = SearchString;

            //var applicationDbContext = _context.WarrantyCard.Include(w => w.Item);
            var applicationDbContext = from w in _context.WarrantyCard
                                       .Include("Item")
                                       select w;
            int count = applicationDbContext.Count();

            //SearchString.ToString() != "" && SearchString.ToString() != "0"
            if (!String.IsNullOrEmpty(SearchString))
            {
                applicationDbContext = applicationDbContext.Where(w => w.Item.IMEI == SearchString);
            }
            
            switch(sortOrder)
            {
                case "DateEnd":
                    applicationDbContext = applicationDbContext.OrderBy(w => w.EndDate);
                    break;
                case "dateend_desc":
                    applicationDbContext = applicationDbContext.OrderByDescending(w => w.EndDate);
                    break;
                case "DateStart":
                    applicationDbContext = applicationDbContext.OrderBy(w => w.StartDate);
                    break;
                default:
                    applicationDbContext = applicationDbContext.OrderByDescending(w => w.StartDate);
                    break;
            }
            int pageSize = 20;
            int count2 = applicationDbContext.Count();
            return View(await PaginatedList<WarrantyCard>.CreateAsync(applicationDbContext.AsNoTracking(), page ?? 1, pageSize));
            //return View(await applicationDbContext.ToListAsync());
        }

        // GET: WarrantyCards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warrantyCard = await _context.WarrantyCard
                .Include(w => w.Item)
                .SingleOrDefaultAsync(m => m.WarrantyCardID == id);
            if (warrantyCard == null)
            {
                return NotFound();
            }

            return View(warrantyCard);
        }

        // GET: WarrantyCards/Create
        public IActionResult Create()
        {
            ViewData["ItemID"] = new SelectList(_context.Item, "ItemID", "IMEI");
            return View();
        }

        // POST: WarrantyCards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("WarrantyCardID,NumberOfWarranty,StartDate,EndDate,Period,ItemID")] WarrantyCard warrantyCard)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(warrantyCard);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ItemID"] = new SelectList(_context.Item, "ItemID", "Name", warrantyCard.ItemID);
        //    return View(warrantyCard);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WarrantyCardID,Period")] WarrantyCard warrantyCard)
        {
            //kiem tra item nhap vao co ton tai trong co so du lieu item
            string imei = Request.Form["ItemID"].ToString();
            int count = 0;
            List<Item> item = _context.Item.ToList();
            for(int i=0;i<item.Count();i++)
            {
                if(item[i].IMEI != imei)
                {
                    count = count + 1;
                }
            }

            if(count == item.Count())
            {
                ViewData["erro"] = "Item không tồn tại";
                return View(warrantyCard);
            }

            //tim Item voi Imei tuong ung

            for (int i = 0; i < item.Count(); i++)
            {
                if (item[i].IMEI == imei)
                {
                    warrantyCard.ItemID = item[i].ItemID;
                }
            }


            //tao ngay ket thuc 
            warrantyCard.StartDate  = DateTime.Now;

            var getdate = DateTime.Now.Day;
            var getmounth = DateTime.Now.Month;
            var getyear = DateTime.Now.Year;

            int period = Convert.ToInt32(Request.Form["Period"].ToString());
            var getmounthend = getmounth + period;
            var getyearend = getyear;
            while(getmounthend>12)
            {
                getmounthend = getmounthend - 12;
                getyearend = getyearend + 1;
            }

            warrantyCard.EndDate = new DateTime(getyearend, getmounthend, getdate);

            warrantyCard.ApplicationUserID = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                _context.Add(warrantyCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemID"] = new SelectList(_context.Item, "ItemID", "Name", warrantyCard.ItemID);
            return View(warrantyCard);
        }

        // GET: WarrantyCards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warrantyCard = await _context.WarrantyCard.Include(w=>w.Item).SingleOrDefaultAsync(m => m.WarrantyCardID == id);
            if (warrantyCard == null)
            {
                return NotFound();
            }
            ViewData["ItemIDOld"] = new SelectList(_context.Item, "ItemID", "IMEI", warrantyCard.ItemID);
            ViewData["ItemIDNew"] = warrantyCard.Item.IMEI;
            return View(warrantyCard);
        }

        
        // POST: WarrantyCards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WarrantyCardID,ApplicationUserID,StartDate,Period")] WarrantyCard warrantyCard)
        {


            if (id != warrantyCard.WarrantyCardID)
            {
                return NotFound();
            }


            string imei = Request.Form["ItemID"].ToString();

            int count = 0;
            List<Item> item = _context.Item.ToList();
            for (int i = 0; i < item.Count(); i++)
            {
                if (item[i].IMEI != imei)
                {
                    count = count + 1;
                }
            }

            if (count == item.Count())
            {
                ViewData["erro"] = "Item không tồn tại";
                return View(warrantyCard);
            }

            //tim Item voi Imei tuong ung
           
            for(int i=0;i<item.Count();i++)
            {
                if(item[i].IMEI==imei)
                {
                    warrantyCard.ItemID = item[i].ItemID;
                }
            }



            //sua ngay ket thuc
            var stardate = warrantyCard.StartDate;

            int period = Convert.ToInt32(Request.Form["Period"].ToString());

            int endday = stardate.Day;
            int endmounth = stardate.Month + period;
            int endyear = stardate.Year;

            while(endmounth >12)
            {
                endmounth = endmounth - 12;
                endyear = endyear + 1;
            }

            warrantyCard.EndDate = new DateTime(endyear, endmounth, endday);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(warrantyCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarrantyCardExists(warrantyCard.WarrantyCardID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemID"] = new SelectList(_context.Item, "ItemID", "Name", warrantyCard.ItemID);
            return View(warrantyCard);
        }

        // GET: WarrantyCards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warrantyCard = await _context.WarrantyCard
                .Include(w => w.Item)
                .SingleOrDefaultAsync(m => m.WarrantyCardID == id);
            if (warrantyCard == null)
            {
                return NotFound();
            }

            return View(warrantyCard);
        }

        // POST: WarrantyCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var warrantyCard = await _context.WarrantyCard.SingleOrDefaultAsync(m => m.WarrantyCardID == id);
            _context.WarrantyCard.Remove(warrantyCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WarrantyCardExists(int id)
        {
            return _context.WarrantyCard.Any(e => e.WarrantyCardID == id);
        }
    }
}
