using CarRentalApp.Data;
using CarRentalApp.Models;
using CarRentalApp.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Controllers
{
    
    public class DamageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;


        public DamageController(AppDbContext context, UserManager<AppUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [CustomAuthorize(Roles = "Admin,Customer,Staff")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userType = await _userManager.GetRolesAsync(user);

            List<Damage> damages = new List<Damage>();

            if (userType.Contains("Admin") || userType.Contains("Staff"))
            {
                damages = await _context.Damages
                    .Include(d => d.User)
                    .Include(d => d.Car)
                    .Include(d => d.RentalRequest)
                    .ToListAsync();
            }
            else if (userType.Contains("Customer"))
            {
                damages = await _context.Damages
                    .Include(d => d.User)
                    .Include(d => d.Car)
                    .Include(d => d.RentalRequest)
                    .Where(d => d.UserID == user.Id)
                    .ToListAsync();
            }

            ViewData["UserType"] = userType.FirstOrDefault();

            return View(damages);
        }

        [CustomAuthorize(Roles = "Customer")]
        public IActionResult Create(int reqID, string userID, int carID)
        {
            var viewModel = new DamageViewModel();
            viewModel.ReqID = reqID;
            viewModel.UserID = userID;
            viewModel.CarID = carID;
            return View(viewModel);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DamageViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var damage = new Damage();
                damage.UserID = viewModel.UserID;
                damage.CarID = viewModel.CarID;
                damage.RentalID = viewModel.ReqID;
                damage.DamageDescription = viewModel.DamageDescription;
                damage.DamageStatus = DamageStatus.Reported;
                damage.DamageRequestDate = DateTime.Now;
                damage.PaidDate = null;
                damage.TotalCost = 0;

                _context.Add(damage);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Damage");
            }

            return View(viewModel);
        }
        [HttpGet]
        [CustomAuthorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update(int id)
        {
            var damage = await _context.Damages.FindAsync(id);

            if (damage == null)
            {
                return NotFound();
            }

            var viewModel = new DamageUpdateViewModel
            {
                TotalCost = 0,
                PaymentDeadline = null
            };

            return View(viewModel);
        }


        [HttpPost]
        [CustomAuthorize(Roles = "Admin,Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, DamageUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var damage = await _context.Damages
                    .Include(d => d.User)
                    .Include(d => d.Car)
                    .Include(d => d.RentalRequest)
                    .FirstOrDefaultAsync(d => d.DamageID == id);

            if (damage == null)
            {
                return NotFound();
            }

            damage.TotalCost = viewModel.TotalCost;
            damage.PaymentDeadline = viewModel.PaymentDeadline;
            damage.DamageStatus = DamageStatus.PendingPayment;

            await _context.SaveChangesAsync();

            // send email to user
            var user = await _context.Users.FindAsync(damage.UserID);
            if (user == null)
            {
                return NotFound();
            }
            var subject = $"Pending Payment for Damage of {damage.Car.Manufacturer} {damage.Car.Model}";

            var message = $"<html><body>" +
                $"<p>Dear {user?.FirstName},</p>" +
                $"<p>We hope this email finds you well. On behalf of Hajur Ko Car Rental, we would like to remind you of the pending payment of <strong>Rs. {damage.TotalCost}</strong> for the rental of <strong>{damage.Car.Manufacturer} {damage.Car.Model}</strong>.</p>" +
                $"<p>Please note that the payment deadline is <strong>{damage.PaymentDeadline:dd MMMM yyyy}</strong>, and we kindly request that you settle the outstanding balance as soon as possible to avoid any inconvenience. Please refer to the payment details mentioned in the initial rental agreement or contact us if you require any further information.</p>" +
                $"<p>At Hajur Ko Car Rental, we value our customers' satisfaction and aim to continue providing you with excellent service. Your prompt attention to this matter would be greatly appreciated.</p>" +
                $"<p>Thank you for choosing Hajur Ko Car Rental, and we look forward to hearing from you soon.</p>" +
                $"<p>Best regards,</p>" +
                $"<p>Hajur Ko Car Rental</p>" +
                $"</body></html>";



            await _emailSender.SendEmailAsync(user.Email, subject, message);
            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Paid(int id)
        {
            var damage = await _context.Damages.FindAsync(id);

            if (damage == null)
            {
                return NotFound();
            }

            damage.DamageStatus = DamageStatus.Paid;
            damage.PaidDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Damage");
        }

        [CustomAuthorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> SendMail(int id)
        {
            var damage = await _context.Damages
                .Include(d => d.User)
                .Include(d => d.Car)
                .Include(d => d.RentalRequest)
                .FirstOrDefaultAsync(d => d.DamageID == id);

            if (damage == null)
            {
                return NotFound();
            }

            if (damage.PaymentDeadline > DateTime.Now)
            {
                return RedirectToAction("Index","Damage");
            }

            // send email to user
            var user = await _context.Users.FindAsync(damage.UserID);
            if (user == null)
            {
                return NotFound();
            }

            var subject = $"Reminder: Pending Payment for Damage of {damage.Car.Manufacturer} {damage.Car.Model}";

            var message = $"<html><body>" +
                $"<p>Dear {user?.FirstName},</p>" +
                $"<p>We hope this email finds you well. On behalf of Hajur Ko Car Rental, we would like to remind you of the pending payment of <strong>Rs. {damage.TotalCost}</strong> for the rental of <strong>{damage.Car.Manufacturer} {damage.Car.Model}</strong>." +
                $"We regret to inform you that the payment deadline of <strong>{damage.PaymentDeadline:dd MMMM yyyy}</strong> has passed, and the outstanding balance has not been settled yet.</p>" +
                $"<p>Please note that failure to settle the outstanding balance may result in additional charges and legal action, as stated in the initial rental agreement. If you have any concerns or require assistance, please do not hesitate to contact us immediately.</p>" +
                $"<p>At Hajur Ko Car Rental, we value our customers' satisfaction and aim to continue providing you with excellent service. Your prompt attention to this matter would be greatly appreciated." +
                $"Thank you for choosing Hajur Ko Car Rental, and we look forward to hearing from you soon.</p>" +
                $"<p>Best regards,</p>" +
                $"<p>Hajur Ko Car Rental</p>" +
                $"</body></html>";

            await _emailSender.SendEmailAsync(user.Email, subject, message);

            TempData["Message"] = "Deadline passed email sent to user";
            return RedirectToAction("Index", "Damage");
        }

    }
}
