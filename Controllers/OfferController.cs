using CarRentalApp.Data;
using CarRentalApp.Models;
using CarRentalApp.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace CarRentalApp.Controllers
{

    [CustomAuthorize(Roles = "Admin,Staff")]
    public class OfferController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;

        public OfferController(UserManager<AppUser> userManager, AppDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        // GET: Offer
        public async Task<ActionResult> Index()
        {
            List<Offer> offers = await _db.Offers.Include(o => o.Car).ToListAsync();
            return View(offers);
        }

        // GET: Offer/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var cars = await _db.Cars.Select(c => new { ID = c.CarID, Name = c.Model + "-" + c.Color }).ToListAsync();
            ViewBag.CarID = new SelectList(cars, "ID", "Name");
            return View();
        }

        // POST: Offer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OfferViewModel offerViewModel)
        {
            if (ModelState.IsValid)
            {
                var offer = new Offer
                {
                    CarID = offerViewModel.CarID,
                    DiscountRate = offerViewModel.DiscountRate,
                    OfferDescription = offerViewModel.OfferDescription,
                    OfferEndDate = offerViewModel.OfferEndDate,
                    IsValid = true
                };

                // ensure that OfferEndDate is not today or less
                if (offer.OfferEndDate <= DateTime.Today)
                {
                    ModelState.AddModelError("OfferEndDate", "Offer end date must be greater than today.");
                    var carsF = await _db.Cars.Select(c => new { ID = c.CarID, Name = c.Model + "-" + c.Color }).ToListAsync();
                    ViewBag.CarID = new SelectList(carsF, "ID", "Name", offer.CarID);
                    return View(offerViewModel);
                }

                _db.Offers.Add(offer);
                await _db.SaveChangesAsync();

                // Send email to customer user
                var customerUsers = await _userManager.GetUsersInRoleAsync("Customer");
                var car = await _db.Cars.FindAsync(offer.CarID);
                foreach (var user in customerUsers)
                {
                    var subject = "New Offer Available";
                    var message = $"Dear {user?.FirstName} {user?.LastName},<br><br>A new offer is now available for the following car:<br>{car.Manufacturer} {car.Model} {car.Color}<br><br>Offer Details:<br>{offer.OfferDescription}<br><br>Thank you for using our services!";
                    await _emailSender.SendEmailAsync(user.Email, subject, message);
                }

                return RedirectToAction("Index", "Offer");
            }

            ModelState.AddModelError("", "The offer details are invalid.");
            var cars = await _db.Cars.Select(c => new { ID = c.CarID, Name = c.Model + "-" + c.Color }).ToListAsync();
            ViewBag.CarID = new SelectList(cars, "ID", "Name", offerViewModel.CarID);
            return View(offerViewModel);
        }



        // GET: Offer/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Offer offer = await _db.Offers.FindAsync(id);

            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // POST: Offer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Offer offer = await _db.Offers.FindAsync(id);
            _db.Offers.Remove(offer);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
