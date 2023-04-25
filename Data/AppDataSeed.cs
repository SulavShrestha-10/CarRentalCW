using CarRentalApp.Models;
using CarRentalApp.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.Drawing;

namespace CarRentalApp.Data
{
    public class AppDataSeed
    {
        public static async Task SeedAdminAndStaffAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {

                // Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                var adminUserEmail = "admin@carrental.com";
                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new AppUser
                    {
                        FirstName = "Seed",
                        LastName = "Admin",
                        UserName = adminUserEmail,
                        Email = adminUserEmail,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAdminUser, "CarRental@1234?");
                    await userManager.AddToRoleAsync(newAdminUser, UserRole.Admin);
                }

                var staffUserEmail = "staff@carrental.com";
                var staffUser = await userManager.FindByEmailAsync(staffUserEmail);
                if (staffUser == null)
                {
                    var newStaffUser = new AppUser
                    {
                        FirstName = "Seed",
                        LastName = "Staff",
                        UserName = staffUserEmail,
                        Email = staffUserEmail,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newStaffUser, "CarRental@1234?");
                    await userManager.AddToRoleAsync(newStaffUser, UserRole.Staff);
                }
            }
        }
        public static void SeedCarData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                context.Database.EnsureCreated();

                // Actors 
                if (!context.Cars.Any())
                {
                    context.Cars.AddRange(new List<Car>
                    {
                        new()
                        {
                            Manufacturer ="Honda",
                            Model = "Civic",
                            Color ="White",
                            RentalRate = 25000,
                            VehicleNo = "Ba 1 Ja 1234",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/Kc030f1h/Hionda-Civic.jpg",
                        },
                        new()
                        {
                            Manufacturer ="Honda",
                            Model = "Accord",
                            Color ="White",
                            RentalRate = 15000,
                            VehicleNo = "Be 2 Ba 5678",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/WzYnjPSM/Honda-Accord.jpg",

                        },
                        new()
                        {

                            Manufacturer ="Hyundai",
                            Model = "Alazar",
                            Color ="Black",
                            RentalRate = 20000,
                            VehicleNo = "Ka 3 Ch 9012",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/DwXgVHGX/Hyundai-Alcazar.jpg",

                        },
                        new()
                        {

                            Manufacturer ="Hyundai",
                            Model = "Elantra",
                            Color ="Blue",
                            RentalRate = 18000,
                            VehicleNo = "Me 4 Mo 3456",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/C55GcQYd/Hyundai-elantra.jpg",

                        },
                        new()
                        {

                            Manufacturer ="Kia",
                            Model = "Seltos",
                            Color ="Red",
                            RentalRate = 30000,
                            VehicleNo = "Na 5 Bu 7890",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/xdPLn21c/Kia-Seltos.jpg",

                        },
                        new()
                        {
                            Manufacturer ="Suzuki",
                            Model = "Brezza",
                            Color ="Red",
                            RentalRate = 35000,
                            VehicleNo = "Pa 6 Pe 2345",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/7PR0xXwM/Maruti-Suzuki-Brezza.jpg",
                        },
                        new()
                        {
                            Manufacturer ="Suzuki",
                            Model = "Swift",
                            Color ="Red",
                            RentalRate = 20000,
                            VehicleNo = "Ra 7 Ta 6789",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/WtRd0W1B/Maruti-Suzuki-Swift.jpg",

                        },
                        new()
                        {

                            Manufacturer ="Nissan",
                            Model = "Almera",
                            Color ="Brownish Grey",
                            RentalRate = 28000,
                            VehicleNo = "Sa 8 Ga 0123",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/mk7HpfJL/Nissan-almera-brownish-grey.jpg",

                        },
                        new()
                        {

                            Manufacturer ="Nissan",
                            Model = "Altima",
                            Color ="Silver",
                            RentalRate = 25000,
                            VehicleNo = "Ba 9 Do 4567",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/tJdstdn7/nissan-altima.jpg",

                        },
                        new()
                        {
                            Manufacturer ="Tata",
                            Model = "Altroz",
                            Color ="Golden",
                            RentalRate = 30000,
                            VehicleNo = "Ga 10 Ja 8901",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/FRnHmWtM/tata-altroz.jpg",

                        },
                        new()
                        {

                            Manufacturer ="Tata",
                            Model = "Punch",
                            Color ="Camo",
                            RentalRate = 27000,
                            VehicleNo = "Lu 11 Mi 2345",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/JhrrXVRw/Tata-Punch-Camo.jpg",

                        },
                        new()
                        {

                            Manufacturer ="Tata",
                            Model = "Tiago",
                            Color ="Red",
                            RentalRate = 20000,
                            VehicleNo = "Dh 12 La 6789",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/MTcvc2Y6/Tata-Tiago.jpg",

                        },
                        new()
                        {

                            Manufacturer ="Toyota",
                            Model = "Corolla",
                            Color ="Silver",
                            RentalRate = 18000,
                            VehicleNo = "Bh 13 Ka 0123",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/fWmR1v2r/toyota-corolla.jpg",

                        },
                        new()
                        {
                            Manufacturer ="Toyota",
                            Model = "Land Crusier",
                            Color ="White",
                            RentalRate = 30000,
                            VehicleNo = "Ra 14 Ta 4567",
                            IsAvailable = true,
                            CarImageURL="https://i.postimg.cc/JhSR2yb8/toyota-land-cruiser.jpg",

                        },
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
