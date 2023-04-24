using System.ComponentModel.DataAnnotations;

namespace CarRentalApp.Models
{
    public class ConfirmEmailViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }
    }
}