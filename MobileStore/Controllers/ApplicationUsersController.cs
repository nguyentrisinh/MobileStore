using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MobileStore.Data;
using MobileStore.Models;
using MobileStore.Models.ApplicationUserViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MobileStore.Services;
using Microsoft.Extensions.Logging;


namespace MobileStore.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public ApplicationUsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _context = context;
        }

            // GET: ApplicationUsers
            public async Task<IActionResult> Index(string sortOrder, string currentFilter, int? page, int? pageSize)
        {
            // ViewData["NameSortParm"] is not the param for current sort but the sortOrder for the next sort 
            // If sortOrder is null or empty => current will sort NameAscending => Next sort of FirstName is first_name_desc => ViewData["NameSortParm"] = first_name_desc
            ViewData["FirstNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "first_name_desc" : "";
            ViewData["LastNameSortParm"] = sortOrder == "last_name_asc" ? "last_name_desc" : "last_name_asc";
            ViewData["PhoneSortParm"] = sortOrder == "phone_asc" ? "phone_desc" : "phone_asc";
            ViewData["BirthdaySortParm"] = sortOrder == "birthday_asc" ? "birthday_desc" : "birthday_asc";
            ViewData["AddressSortParm"] = sortOrder == "address_asc" ? "address_desc" : "address_asc";
            ViewData["RoleSortParm"] = sortOrder == "role_asc" ? "role_desc" : "role_asc";
            ViewData["EmailSortParm"] = sortOrder == "email_asc" ? "email_desc" : "email_asc";

            var applicationUsers = from ent in _context.ApplicationUser
                           select ent;

            // Search method
            ViewData["CurrentFilter"] = currentFilter;
            if (!String.IsNullOrEmpty(currentFilter))
            {
                applicationUsers = applicationUsers.Where(ent => ent.FirstName.Contains(currentFilter)
                                                    || ent.LastName.Contains(currentFilter) || ent.Phone.Contains(currentFilter) || ent.Address.Contains(currentFilter) 
                                                    || ent.Email.Contains(currentFilter));
            }

            // Order with sortOrder
            switch (sortOrder)
            {
                case "first_name_desc":
                    applicationUsers = applicationUsers.OrderByDescending(s => s.FirstName);
                    break;
                case "last_name_desc":
                    applicationUsers = applicationUsers.OrderByDescending(s => s.LastName);
                    break;
                case "last_name_asc":
                    applicationUsers = applicationUsers.OrderBy(s => s.LastName);
                    break;
                case "birthday_asc":
                    applicationUsers = applicationUsers.OrderBy(s => s.Birthday);
                    break;
                case "birthday_desc":
                    applicationUsers = applicationUsers.OrderByDescending(s => s.Birthday);
                    break;
                case "phone_asc":
                    applicationUsers = applicationUsers.OrderBy(s => s.Phone);
                    break;
                case "phone_desc":
                    applicationUsers = applicationUsers.OrderByDescending(s => s.Phone);
                    break;
                case "address_asc":
                    applicationUsers = applicationUsers.OrderBy(s => s.Address);
                    break;
                case "address_desc":
                    applicationUsers = applicationUsers.OrderByDescending(s => s.Address);
                    break;
                case "role_asc":
                    applicationUsers = applicationUsers.OrderBy(s => s.Role);
                    break;
                case "role_desc":
                    applicationUsers = applicationUsers.OrderByDescending(s => s.Role);
                    break;
                case "email_asc":
                    applicationUsers = applicationUsers.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    applicationUsers = applicationUsers.OrderByDescending(s => s.Email);
                    break;
                default:
                    applicationUsers = applicationUsers.OrderBy(s => s.FirstName);
                    break;
            }

            return View(await PaginatedList<ApplicationUser>.CreateAsync(applicationUsers.AsNoTracking(), page ?? 1, pageSize ?? 10));

        }

        // GET: ApplicationUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,Password,ConfirmPassword,FirstName,LastName,Birthday,Phone,Address,Role")] CreateApplicationUserViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = ConvertCreateApplicationUserViewModelToApplicationUserModel(model);
                //var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var isCreatedUserRole = await CreateNewUserRole(user, model.Role);

                    if (isCreatedUserRole)
                    {
                        _logger.LogInformation("Create UserRole successfully");
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToAction(nameof(Index));
                }

                AddErrors(result);
            }

            return View(model);
        }

        #region GET: ApplicationUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            return View(applicationUser);
        }
        #endregion

        #region POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("FirstName,LastName,Birthday,Phone,Address,Role,Id,Email,ConcurrencyStamp")] ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    // AsNoTracking to get oldApplicationUser fix "this error The instance of entity type X cannot be tracked because another instance of this type with the same key is already being tracked"
                    // Error occur because Use 2 query first is oldApplicationUser and second is when _context.Update(applicationUser)
                    var oldApplicationUser = await _context.ApplicationUser.AsNoTracking().SingleOrDefaultAsync(m => m.Id == applicationUser.Id);

                    // Remove old Role to update to new role
                    await RemoveCurrentUserRole(applicationUser, oldApplicationUser.Role);

                    // ---------------------Update new role for the ApplicationUser---------------------------
                    var isCreatedUserRole = await CreateNewUserRole(applicationUser, applicationUser.Role);

                    if (isCreatedUserRole)
                    {
                        _logger.LogInformation("Create UserRole successfully");
                    }

                    
                    // -------------------------------------------End-----------------------------------------

                    _context.Update(applicationUser);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUser.Id))
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
            return View(applicationUser);
        }
        #endregion

        #region GET: ApplicationUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUser
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }
        #endregion

        #region POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == id);
            _context.ApplicationUser.Remove(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion


        #region Support Function for ApplicationUserController
        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUser.Any(e => e.Id == id);
        }

        #region Convert Models
        private ApplicationUser ConvertCreateApplicationUserViewModelToApplicationUserModel(CreateApplicationUserViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Birthday = model.Birthday,
                Phone = model.Phone,
                PhoneNumber = model.Phone,
                Address = model.Address,
                Role = model.Role,
            };

            return user;
        }


        #endregion

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private async Task NewUserRole(ApplicationUser user, string role)
        {
            // Add role to the AspRole table
            // Check if the role is exists
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            // Add current user to role 
            await _userManager.AddToRoleAsync(user, role);
        }

        private async Task<bool> RemoveCurrentUserRole(ApplicationUser user, UserRole role)
        {
            try
            {
                switch (role)
                {
                    case UserRole.Admin:
                        await _userManager.RemoveFromRoleAsync(user, "Admin");
                        break;
                    case UserRole.WarehouseManager:
                        await _userManager.RemoveFromRoleAsync(user, "WarehoustManager");
                        break;
                    case UserRole.Technical:
                        await _userManager.RemoveFromRoleAsync(user, "Technical");
                        break;
                    case UserRole.Accountant:
                        await _userManager.RemoveFromRoleAsync(user, "Accountant");
                        break;
                    case UserRole.Sale:
                        await _userManager.RemoveFromRoleAsync(user, "Sale");
                        break;
                }

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return false;
            }
        }

        private async Task<bool> CreateNewUserRole(ApplicationUser user, UserRole role)
        {
            try
            {
                switch (role)
                {
                    case UserRole.Admin:
                        await NewUserRole(user, "Admin");
                        break;
                    case UserRole.WarehouseManager:
                        await NewUserRole(user, "WarehoustManager");
                        break;
                    case UserRole.Technical:
                        await NewUserRole(user, "Technical");
                        break;
                    case UserRole.Accountant:
                        await NewUserRole(user, "Accountant");
                        break;
                    case UserRole.Sale:
                        await NewUserRole(user, "Sale");
                        break;
                }

                return true;

            } catch(Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return false;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        #endregion

    }
}
