﻿using CarRentalApp.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using CarRentalApp.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace CarRentalApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public CustomerController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        [CustomAuthorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "UserUtils");
            }
            var customers = _userManager.GetUsersInRoleAsync(UserRole.Customer).Result;
            return View(customers);
        }
        public IActionResult ConfirmEmailReminder()
        {
            return View();
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
                var account = new Account(
                    "niwahang",
                    "795422516494254",
                    "ydyImVZdZAVjunXmkcaWPiNTcKA");
                var cloudinary = new Cloudinary(account);
                var licenseUploadResult = new ImageUploadResult();
                var citizenshipUploadResult = new ImageUploadResult();
                if (model.CitizenshipImage != null)
                {
                    var citizenshipUploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(model.CitizenshipImage.FileName, model.CitizenshipImage.OpenReadStream())
                    };
                    citizenshipUploadResult = await cloudinary.UploadAsync(citizenshipUploadParams);
                }

                if (model.LicenseImage != null)
                {

                    var licenseUploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(model.LicenseImage.FileName, model.LicenseImage.OpenReadStream())
                    };
                    licenseUploadResult = await cloudinary.UploadAsync(licenseUploadParams);
                }

                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    IsRegular = false,
                    Discount = 0,
                    IsActive = false,
                    CitizenshipURL = citizenshipUploadResult.SecureUrl?.ToString(),
                    DrivingLicenseURL = licenseUploadResult.SecureUrl?.ToString(),

                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, UserRole.Customer);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Customer", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your account", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    return RedirectToAction("Login", "UserUtils");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            return result.Succeeded ? RedirectToAction("Login", "UserUtils") : View("Error");
        }
        [CustomAuthorize(Roles = "Customer")]
        public async Task<IActionResult> AddImages()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new AddImagesViewModel();
            if (!string.IsNullOrEmpty(user.CitizenshipURL))
            {
                model.CitizenshipImageExists = true;
            }
            if (!string.IsNullOrEmpty(user.DrivingLicenseURL))
            {
                model.DrivingLicenseImageExists = true;
            }

            return View(model);
        }
        [HttpPost]
        [CustomAuthorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImages(AddImagesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var account = new Account(
                    "niwahang",
                    "795422516494254",
                    "ydyImVZdZAVjunXmkcaWPiNTcKA");
                var cloudinary = new Cloudinary(account);
                var user = await _userManager.GetUserAsync(User);

                if (!model.CitizenshipImageExists && model.CitizenshipImage != null)
                {
                    var citizenshipUploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(model.CitizenshipImage.FileName, model.CitizenshipImage.OpenReadStream())
                    };
                    var citizenshipUploadResult = await cloudinary.UploadAsync(citizenshipUploadParams);
                    user.CitizenshipURL = citizenshipUploadResult.SecureUrl?.ToString();
                }

                if (!model.DrivingLicenseImageExists && model.DrivingLicenseImage != null)
                {
                    var licenseUploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(model.DrivingLicenseImage.FileName, model.DrivingLicenseImage.OpenReadStream())
                    };
                    var licenseUploadResult = await cloudinary.UploadAsync(licenseUploadParams);
                    user.DrivingLicenseURL = licenseUploadResult.SecureUrl?.ToString();
                }

                await _userManager.UpdateAsync(user);

                return RedirectToAction("Index", "UserUtils");
            }

            return View(model);
        }

    }
}