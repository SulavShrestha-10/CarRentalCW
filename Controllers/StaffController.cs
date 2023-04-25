using CarRentalApp.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using CarRentalApp.Models;
using NuGet.Packaging;

namespace CarRentalApp.Controllers
{

    [Authorize(Roles = UserRole.Staff + "," + UserRole.Admin)]
    public class StaffController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public StaffController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var adminAndStaff = await _userManager.GetUsersInRoleAsync(UserRole.Admin);
            adminAndStaff.AddRange(await _userManager.GetUsersInRoleAsync(UserRole.Staff));
            ViewBag.UserManager = _userManager;
            return View(adminAndStaff.ToList());
        }


        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, IsRegular = false, Discount = 0, IsActive = false };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Set email confirmed status to true
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                    // Check user type before adding to role
                    if (model.UserType == UserRole.Admin)
                    {
                        await _userManager.AddToRoleAsync(user, UserRole.Admin);
                    }
                    else if (model.UserType == UserRole.Staff)
                    {
                        await _userManager.AddToRoleAsync(user, UserRole.Staff);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }


    }
}
