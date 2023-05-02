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
            };

            return View(viewModel);
        }


        [HttpPost]
        [CustomAuthorize(Roles = "Admin,Staff,Customer")]
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


            user.FirstName = viewModel.FirstName;
            user.LastName = viewModel.LastName;
            user.PhoneNumber = viewModel.PhoneNumber;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        [CustomAuthorize(Roles = "Admin,Staff,Customer")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin,Staff,Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound();
                }

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [CustomAuthorize(Roles = "Customer,Admin,Staff")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Customer,Admin,Staff")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                await _userManager.DeleteAsync(user);
                return RedirectToAction("Index", "Staff");
            }
            else
            {
                await _userManager.DeleteAsync(user);
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
