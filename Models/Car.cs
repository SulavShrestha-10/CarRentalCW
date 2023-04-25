using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CarRentalApp.Models
{
    public class Car
    {
        [Key] public int CarID { get; set; }
        public string? Manufacturer { get; set; }

        public string? Model { get; set; }
        public string? Color { get; set; }
        public decimal RentalRate { get; set; }
        public string? VehicleNo { get; set; }
        public bool IsAvailable { get; set; }
        public string? CarImageURL { get; set; }

        [JsonIgnore]
        public ICollection<Offer>? Offers { get; set; }
        [JsonIgnore]
        public ICollection<RentalRequest>? RentalRequests { get; set; }
        [JsonIgnore]
        public ICollection<Damage>? Damages { get; set; }
        [JsonIgnore]
        public List<RentalHistory>? RentalHistories { get; set; }

    }
}
