using CarRentalApp.Data;
using CarRentalApp.Models;
using CarRentalApp.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace CarRentalApp.Controllers
{
    public class UserUtilsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;

        public UserUtilsController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    await UpdateIsRegular(user.Id);
                    await UpdateIsActive(user.Id, DateTime.Today.AddDays(-90));

                    await _userManager.UpdateAsync(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public async Task<IActionResult> UpdateIsRegular(string userId, int rentalCountThreshold = 5)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var rentalCount = await _context.RentalHistories
                                            .CountAsync(r => r.UserID == userId);

            user.IsRegular = rentalCount >= rentalCountThreshold;
            if (user.IsRegular)
            {
                user.Discount = 10;
            }
            await _userManager.UpdateAsync(user);

            return Ok();
        }

        public async Task<IActionResult> UpdateIsActive(string userId, DateTime lastRentedDateThreshold)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = await _context.RentalHistories
                                         .AnyAsync(r => r.UserID == userId && r.RentalDate >= lastRentedDateThreshold);

            await _userManager.UpdateAsync(user);

            return Ok();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProfileUpdate(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserProfileModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                Password = "",
                DrivingLicenseURL = user.DrivingLicenseURL,
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfileUpdate(string id, UserProfileModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _context.Users
                    .FirstOrDefaultAsync(d => d.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            var hashedPassword = _userManager.PasswordHasher.HashPassword(user, viewModel.Password);

            user.FirstName = viewModel.FirstName;
            user.LastName = viewModel.LastName;
            user.PasswordHash = hashedPassword;
            user.PhoneNumber = viewModel.PhoneNumber;
            user.UserName = viewModel.UserName;
            user.DrivingLicenseURL = viewModel.DrivingLicenseURL;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
