using Microsoft.AspNetCore.Identity;

namespace CarRentalApp.Models.Identity
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsRegular { get; set; }
        public int Discount { get; set; }
        public bool IsActive { get; set; }
        public string? DrivingLicenseURL { get; set; }
        public string? CitizenshipURL { get; set; }
        public ICollection<RentalRequest> RentalRequests { get; set; }
        public ICollection<RentalHistory> RentalHistories { get; set; }
        public ICollection<Damage> Damages { get; set; }

    }
}
