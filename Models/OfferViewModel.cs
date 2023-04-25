using System.ComponentModel.DataAnnotations;

public class OfferViewModel
{
    public int OfferID { get; set; }

    [Required(ErrorMessage = "Please select a car.")]
    public int CarID { get; set; }

    [Required(ErrorMessage = "Please enter a discount rate.")]
    [Range(0, 100, ErrorMessage = "Discount rate must be between 0 and 100.")]
    public int DiscountRate { get; set; }

    [Required(ErrorMessage = "Please enter a description.")]
    [StringLength(500, ErrorMessage = "Description must be less than or equal to 500 characters.")]
    public string OfferDescription { get; set; }

    [Required(ErrorMessage = "Please enter an end date.")]
    [Display(Name = "Offer End Date")]
    [DataType(DataType.Date, ErrorMessage = "Please enter a valid date in the format MM/dd/yyyy.")]
    [CustomValidation(typeof(OfferViewModel), "ValidateEndDate")]
    public DateTime OfferEndDate { get; set; }


    public static ValidationResult ValidateEndDate(DateTime endDate)
    {
        if (endDate <= DateTime.Today)
        {
            return new ValidationResult("Offer end date must be greater than today.");
        }
        return ValidationResult.Success;
    }
}
