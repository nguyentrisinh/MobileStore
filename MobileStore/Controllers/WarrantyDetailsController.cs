using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MobileStore.Data;
using MobileStore.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace MobileStore.Controllers
{
    public class WarrantyDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        [HttpPost]
        public async Task<IActionResult> StorageLocations()
        {


            var k = _context.WarrantyCard.ToList();
            List<String> kl = new List<String>();
            for (int i = 0; i < k.Count(); i++)
            {
                kl.Add(k[i].TransactionCode.ToString());
            }
            var json = JsonConvert.SerializeObject(kl);
            return Json(json);
        }

       
        public WarrantyDetailsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: WarrantyDetails
        public async Task<IActionResult> Index(string SearchString, string sortOrder, string currentFilter, int? page)
        {
            ViewData["DateStartSortParm"] = sortOrder == "DateStart" ? "datestart_desc" : "DateStart";
            ViewData["CurrentSort"] = sortOrder;

            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }

            ViewData["CurrentFilter"] = SearchString;

            //var applicationDbContext = _context.WarrantyDetail.Include(w => w.WarrantyCard);

            var applicationDbContext = from wd in _context.WarrantyDetail
                                       .Include("WarrantyCard")
                                       select (wd);
            if(!String.IsNullOrEmpty(SearchString))
            {
                applicationDbContext = applicationDbContext.Where(wd => wd.WarrantyCard.TransactionCode.ToString() == SearchString);
            }
     
                switch(sortOrder)
                {
                    case "datestart_desc":
                        applicationDbContext = applicationDbContext.OrderByDescending(wd => wd.Date);
                        break;
                    default:
                        applicationDbContext = applicationDbContext.OrderBy(wd => wd.Date);
                        break;
                }
           

            int pageSize = 2;
            return View(await PaginatedList<WarrantyDetail>.CreateAsync(applicationDbContext.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: WarrantyDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warrantyDetail = await _context.WarrantyDetail
                .Include(w => w.WarrantyCard)
                .SingleOrDefaultAsync(m => m.WarrantyDetailID == id);
            if (warrantyDetail == null)
            {
                return NotFound();
            }

            return View(warrantyDetail);
        }

        // GET: WarrantyDetails/Create
        public IActionResult Create()
        {
            ViewData["CurrentDate"] = DateTime.Now.ToShortDateString();
            ViewData["WarrantyCardID"] = new SelectList(_context.WarrantyCard, "WarrantyCardID", "TransactionCode");
            return View();
        }

        // POST: WarrantyDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WarrantyDetailID,Date,DefectInfo,Status")] WarrantyDetail warrantyDetail)
        {
            //kiem tra transaction thuoc warrantyCard nao
            string transaction = Request.Form["waCardID"].ToString().Trim();

            var warrantyCard = _context.WarrantyCard.Where(w=>w.TransactionCode.ToString()==transaction).SingleOrDefault();

            //kiem tra waranty card nhap vo co ton tai khon
            List<WarrantyCard> warCart = _context.WarrantyCard.ToList();
            int count = 0;
            for(int i=0;i< warCart.Count();i++)
            {
                if(warCart[i].TransactionCode.ToString()!=transaction)
                {
                    //ViewData["erro"] = "mã bảo hành không tồn tại";
                    //return View(warrantyDetail);
                    count = count+1;
                }
            }
            if(count == warCart.Count())
            {
                ViewData["erro"] = "mã bảo hành không tồn tại";
                return View(warrantyDetail);
            }


            //tang tu dong them 1 khi nguoi dung tao mot chi tiet bao hanh
            //var warrantyCard = _context.WarrantyCard.Where(w => w.WarrantyCardID == warrantyDetail.WarrantyCardID).SingleOrDefault();
            warrantyCard.NumberOfWarranty = warrantyCard.NumberOfWarranty + 1;
            warrantyDetail.ApplicationUserID = _userManager.GetUserId(User);
            warrantyDetail.WarrantyCardID = warrantyCard.WarrantyCardID;
           


            if (ModelState.IsValid)
            {
                _context.Update(warrantyCard);
                _context.Add(warrantyDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           
            //ViewData["WarrantyCardID"] = new SelectList(_context.WarrantyCard, "WarrantyCardID", "TransactionCode", warrantyDetail.WarrantyCardID);
            return View(warrantyDetail);
        }

        // GET: WarrantyDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warrantyDetail = await _context.WarrantyDetail.Include(w=>w.WarrantyCard).SingleOrDefaultAsync(m => m.WarrantyDetailID == id);
            if (warrantyDetail == null)
            {
                return NotFound();
            }
            ViewData["WarrantyCardID"] = new SelectList(_context.WarrantyCard, "WarrantyCardID", "TransactionCode", warrantyDetail.WarrantyCardID);
            //ViewData["EditWarrantyCardID"] = warrantyDetail.WarrantyCard.TransactionCode.ToString();
            return View(warrantyDetail);
        }

        // POST: WarrantyDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WarrantyDetailID,ApplicationUserID,Date,DefectInfo,Status,WarrantyCardID")] WarrantyDetail warrantyDetail)
        {
            if (id != warrantyDetail.WarrantyDetailID)
            {
                return NotFound();
            }

           

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(warrantyDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarrantyDetailExists(warrantyDetail.WarrantyDetailID))
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
            ViewData["WarrantyCardID"] = new SelectList(_context.WarrantyCard, "WarrantyCardID", "TransactionCode", warrantyDetail.WarrantyCardID);
            return View(warrantyDetail);
        }

        // GET: WarrantyDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warrantyDetail = await _context.WarrantyDetail
                .Include(w => w.WarrantyCard)
                .SingleOrDefaultAsync(m => m.WarrantyDetailID == id);
            if (warrantyDetail == null)
            {
                return NotFound();
            }

            return View(warrantyDetail);
        }

        // POST: WarrantyDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var warrantyDetail = await _context.WarrantyDetail.SingleOrDefaultAsync(m => m.WarrantyDetailID == id);
            _context.WarrantyDetail.Remove(warrantyDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WarrantyDetailExists(int id)
        {
            return _context.WarrantyDetail.Any(e => e.WarrantyDetailID == id);
        }
    }
}
