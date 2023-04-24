using CarRentalApp.Models.Identity;
using Microsoft.AspNetCore.Identity;

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
    }
}
