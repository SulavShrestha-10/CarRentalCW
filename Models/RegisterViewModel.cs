﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using CarRentalApp.Models.Identity;
namespace CarRentalApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\+?[0-9]{10}$", ErrorMessage = "Phone number must have exactly 10 digits.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "User Type")]
        public string? UserType { get; set; }
        [Display(Name = "Citizenship Image")]
        public IFormFile? CitizenshipImage { get; set; }
        [Display(Name = "License Image")]
        public IFormFile? LicenseImage { get; set; }

    }
}
