using CarRentalApp.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApp.Models
{
    public class RentalHistory
    {
        [Key] public int RentalID { get; set; }
        [ForeignKey("Id")] public string UserID { get; set; }
        [ForeignKey("CarID")] public int CarID { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        [ForeignKey("Id")] public string? AuthorizedByID { get; set; }
        public virtual AppUser User { get; set; }
        public virtual Car Car { get; set; }
        public virtual AppUser AuthorizedBy { get; set; }

        // Navigation property
        public virtual RentalRequest RentalRequest { get; set; }
    }
}
