using CarRentalApp.Data;
using CarRentalApp.Models;
using CarRentalApp.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarRentalApp.Controllers
{
    public class RentalRequestController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;

        public RentalRequestController(AppDbContext context, UserManager<AppUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userType = await _userManager.GetRolesAsync(user);

            List<RentalRequest> rentalRequests = new List<RentalRequest>();

            if (userType.Contains("Admin") || userType.Contains("Staff"))
            {
                rentalRequests = await _context.RentalRequests
                    .Include(r => r.User)
                    .Include(r => r.Car)
                    .ToListAsync();
            }
            else if (userType.Contains("Customer"))
            {
                rentalRequests = await _context.RentalRequests
                    .Include(r => r.User)
                    .Include(r => r.Car)
                    .Where(r => r.User.Id == user.Id)
                    .ToListAsync();
            }

            ViewData["UserType"] = userType.FirstOrDefault();

            return View(rentalRequests);
        }

        public async Task<IActionResult> Create(int carId)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var rentalRequest = new RentalRequest
                {
                    CarID = carId,
                    UserID = userId,
                    RequestDate = DateTime.Now,
                    Status = RentalRequestStatus.Pending
                };

                _context.RentalRequests.Add(rentalRequest);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View();
        }


        public async Task<IActionResult> Approve(int id)
        {

            var rentalRequest = await _context.RentalRequests
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.ReqID == id);

            if (rentalRequest == null)
            {
                return NotFound();
            }

            rentalRequest.Status = RentalRequestStatus.Approved;

            var authUser = await _userManager.GetUserAsync(User);
            var userId = authUser?.Id;
            var authorizedUser = await _userManager.FindByIdAsync(userId);
            if (authorizedUser == null)
            {
                return NotFound();
            }


            var rentalHistory = new RentalHistory
            {
                CarID = (int)rentalRequest.CarID,
                UserID = rentalRequest.UserID,
                AuthorizedByID = userId,
                RentalDate = rentalRequest.RequestDate,
                TotalCost = rentalRequest.Car.RentalRate,
                RentalRequest = rentalRequest
            };
            rentalRequest.Car.IsAvailable = false;
            _context.RentalHistories.Add(rentalHistory);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(rentalRequest.UserID);
            var emailSubject = "Rental Request Approved";
            var emailMessage = $"Dear {user.FirstName},<br><br>Your rental request for {rentalRequest.Car.Manufacturer} {rentalRequest.Car.Model} has been approved. Please proceed to the rental office to complete the necessary formalities.<br><br>Thank you,<br>Hamro Car Rental Team";
            await _emailSender.SendEmailAsync(user.Email, emailSubject, emailMessage);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(int id)
        {
            var rentalRequest = await _context.RentalRequests
                .Include(rr => rr.Car)
                .FirstOrDefaultAsync(rr => rr.ReqID == id);
            if (rentalRequest == null)
            {
                return NotFound();
            }

            rentalRequest.Status = RentalRequestStatus.Rejected;
            _context.Update(rentalRequest);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(rentalRequest.UserID);
            if (user == null)
            {
                return NotFound();
            }
            if (rentalRequest.Car == null)
            {
                return NotFound();
            }
            var emailSubject = "Rental Request Rejected";
            var emailMessage = $"Dear {user.FirstName},<br><br>Your rental request for {rentalRequest.Car.Manufacturer} {rentalRequest.Car.Model} has been rejected. Please contact our customer service team for more information.<br><br>Thank you,<br>The Car Rental Team";
            await _emailSender.SendEmailAsync(user.Email, emailSubject, emailMessage);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Cancel(int id)
        {
            var rentalRequest = await _context.RentalRequests
                .Include(r => r.User)
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.ReqID == id && r.Status == RentalRequestStatus.Pending);

            if (rentalRequest == null)
            {
                return NotFound();
            }
            rentalRequest.Car.IsAvailable = true;
            rentalRequest.Status = RentalRequestStatus.Canceled;
            _context.RentalRequests.Update(rentalRequest);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
    }
}
