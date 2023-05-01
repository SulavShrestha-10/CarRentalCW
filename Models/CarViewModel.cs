using System.ComponentModel.DataAnnotations;

public class CarViewModel
{
    [Required(ErrorMessage = "The Manufacturer field is required.")]
    public string Manufacturer { get; set; }

    [Required(ErrorMessage = "The Model field is required.")]
    public string Model { get; set; }
    [Required(ErrorMessage = "The Color field is required.")]
    public string Color { get; set; }

    [Required(ErrorMessage = "The Rental Rate field is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to 0.")]
    public decimal RentalRate { get; set; }

    [Required(ErrorMessage = "The Vehicle Number field is required.")]
    public string VehicleNo { get; set; }

    public IFormFile CarImage { get; set; }
}
