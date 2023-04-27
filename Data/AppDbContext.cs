using CarRentalApp.Models;
using CarRentalApp.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CarRentalApp.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<RentalHistory> RentalHistories { get; set; }
        public DbSet<RentalRequest> RentalRequests { get; set; }
        public DbSet<Damage> Damages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "Staff", NormalizedName = "STAFF" },
                new IdentityRole { Id = "3", Name = "Customer", NormalizedName = "CUSTOMER" }
            );
            // Configure relationships

            // 1-to-many relationship between AppUser and RentalRequest
            modelBuilder.Entity<RentalRequest>()
                .HasOne(r => r.User)
                .WithMany(u => u.RentalRequests)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RentalRequest>()
                .HasOne(r => r.AuthorizedByUser)
                .WithMany()
                .HasForeignKey(r => r.AuthorizedBy)
                .OnDelete(DeleteBehavior.Restrict);
            // 1-to-many relationship between Car and Offer
            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Car)
                .WithMany(c => c.Offers)
                .HasForeignKey(o => o.CarID)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-many relationship between Car and RentalRequest
            modelBuilder.Entity<RentalRequest>()
                .HasOne(r => r.Car)
                .WithMany(c => c.RentalRequests)
                .HasForeignKey(r => r.CarID)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-many relationship between Car and Damage
            modelBuilder.Entity<Damage>()
                .HasOne(d => d.Car)
                .WithMany(c => c.Damages)
                .HasForeignKey(d => d.CarID)
                .OnDelete(DeleteBehavior.Cascade);

            // many-to-many relationship between AppUser and Car (through RentalHistory)
            modelBuilder.Entity<RentalHistory>()
                .HasKey(rh => new { rh.UserID, rh.CarID });

            modelBuilder.Entity<RentalHistory>()
                .HasOne(rh => rh.User)
                .WithMany(u => u.RentalHistories)
                .HasForeignKey(rh => rh.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RentalHistory>()
                .HasOne(rh => rh.Car)
                .WithMany(c => c.RentalHistories)
                .HasForeignKey(rh => rh.CarID)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-many relationship between AppUser and RentalHistory (as AuthorizedBy)
            modelBuilder.Entity<RentalHistory>()
                .HasOne(rh => rh.AuthorizedBy)
                .WithMany()
                .HasForeignKey(rh => rh.AuthorizedByID)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-to-many relationship between AppUser and Damage
            modelBuilder.Entity<Damage>()
                .HasOne(d => d.User)
                .WithMany(u => u.Damages)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-many relationship between RentalRequest and RentalHistory
            modelBuilder.Entity<RentalHistory>()
                .HasOne(rh => rh.RentalRequest)
                .WithMany()
                .HasForeignKey(rh => rh.RentalID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RentalRequest>()
                .HasMany(rr => rr.RentalHistories)
                .WithOne(rh => rh.RentalRequest)
                .HasForeignKey(rh => rh.RentalID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Damage>()
                .HasOne(d => d.RentalRequest)
                .WithMany(r => r.Damages)
                .HasForeignKey(d => d.RentalID)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
