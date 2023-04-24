using CarRentalApp.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApp.Models
{
    public class RentalRequest
    {
        [Key] public int ReqID { get; set; }
        [ForeignKey("Id")] public string UserID { get; set; }
        [ForeignKey("CarID")] public int? CarID { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string? Status { get; set; }
        [ForeignKey("Id")] public string? AuthorizedBy { get; set; }
        public virtual AppUser User { get; set; }
        public virtual Car Car { get; set; }
        public virtual AppUser AuthorizedByUser { get; set; }

        // Navigation property
        public virtual ICollection<RentalHistory> RentalHistories { get; set; }
        public virtual ICollection<Damage> Damages { get; set; }
    }
}

