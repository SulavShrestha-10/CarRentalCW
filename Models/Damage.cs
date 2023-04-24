using CarRentalApp.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApp.Models
{
    public class Damage
    {
        [Key] public int DamageID { get; set; }
        [ForeignKey("Id")] public string UserID { get; set; }
        [ForeignKey("CarID")] public int CarID { get; set; }
        [ForeignKey("RentalId")] public int RentalID { get; set; }
        public string? DamageDescription { get; set; }
        public string? DamageStatus { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime DamageRequestDate { get; set; }
        public DateTime PaidDate { get; set; }
        public virtual AppUser User { get; set; }
        public virtual Car Car { get; set; }
        public virtual RentalRequest RentalRequest { get; set; }
    }
}
