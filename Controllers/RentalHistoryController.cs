using CarRentalApp.Data;
using CarRentalApp.Models;
using CarRentalApp.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Controllers
{
    public class RentalHistoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public RentalHistoryController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userType = await _userManager.GetRolesAsync(user);

            List<RentalHistory> rentalHistoryList = new List<RentalHistory>();

            if (userType.Contains("Admin") || userType.Contains("Staff"))
            {
                rentalHistoryList = await _context.RentalHistories
                    .Include(rh => rh.RentalRequest)
                    .Include(rh => rh.User)
                    .Include(rh => rh.Car)
                    .ToListAsync();
            }
            else if (userType.Contains("Customer"))
            {
                rentalHistoryList = await _context.RentalHistories
                    .Include(rh => rh.RentalRequest)
                    .Include(rh => rh.User)
                    .Include(rh => rh.Car)
                    .Where(rh => rh.User.Id == user.Id)
                    .ToListAsync();
            }

            ViewData["UserType"] = userType.FirstOrDefault();

            return View(rentalHistoryList);
        }

        public async Task<IActionResult> Return(int id)
        {
            decimal totalCost;
            var rentalHistory = await _context.RentalHistories
                .Include(rh => rh.RentalRequest)
                .Include(rh => rh.Car)
                .FirstOrDefaultAsync(rh => rh.RentalRequest.ReqID == id && rh.ReturnedDate == null);

            if (rentalHistory == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(rentalHistory.UserID);
            // Determine the rental rate based on whether the user is regular or not
            decimal rentalRate = rentalHistory.Car.RentalRate;
            if (user.IsRegular)
            {
                rentalRate *= 0.9m; // Apply 10% discount for regular users
            }

            // Check if any offers are valid for the rented car and current date
            var currentDate = DateTime.Now.Date;
            var offers = await _context.Offers
                .Where(o => o.CarID == rentalHistory.CarID && o.IsValid == true && o.OfferEndDate >= currentDate)
                .ToListAsync();

            if (offers.Count > 0)
            {
                var offerDiscount = offers[0].DiscountRate / 100;
                rentalRate *= (1 - offerDiscount); // Apply offer discount
            }
            // Calculate total cost using the discounted rental rate
            decimal days = (decimal)(currentDate - rentalHistory.RentalDate).TotalDays;
            decimal rentalDays = days < 1 ? 1 : days;
            totalCost = rentalRate * rentalDays;
            rentalHistory.ReturnedDate = currentDate;
            rentalHistory.TotalCost = totalCost;
            rentalHistory.RentalRequest.Status = RentalRequestStatus.Returned;
            rentalHistory.Car.IsAvailable = true;
            _context.RentalHistories.Update(rentalHistory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
