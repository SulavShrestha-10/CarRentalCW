using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Models
{
    public class UserProfileModel
    {
        public string? Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must contain exactly 10 digits")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}
