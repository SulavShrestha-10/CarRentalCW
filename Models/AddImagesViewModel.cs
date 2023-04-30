using System.ComponentModel.DataAnnotations;


namespace CarRentalApp.Models
{
    public class AddImagesViewModel
    {
        [Required(ErrorMessage = "Please select a citizenship image.")]
        [Display(Name = "Citizenship Image")]
        public IFormFile CitizenshipImage { get; set; }

        [Required(ErrorMessage = "Please select a driving license image.")]
        [Display(Name = "Driving License Image")]
        public IFormFile DrivingLicenseImage { get; set; }
        public bool CitizenshipImageExists { get; set; }
        public bool DrivingLicenseImageExists { get; set; }
    }
}
