﻿using CarRentalApp.Data;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace CarRentalApp.Controllers
{
    public class CarsController : Controller
    {
        private readonly AppDbContext _context;

        public CarsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index(string view = "all")
        {
            IQueryable<Car> query = _context.Cars.AsQueryable();

            switch (view.ToLower())
            {
                case "available":
                    ViewBag.PageTitle = "Available Cars";
                    ViewBag.Heading = "Available Cars";
                    query = query.Where(c => c.IsAvailable == true);
                    break;
                case "onrent":
                    ViewBag.PageTitle = "Cars on Rent";
                    ViewBag.Heading = "Cars on Rent";
                    query = query.Where(c => c.IsAvailable == false);
                    break;
                case "frequentlyrented":
                    ViewBag.PageTitle = "Frequently Rented Cars";
                    ViewBag.Heading = "Frequently Rented Cars";
                    query = query.Where(c => c.RentalRequests.Count > 0)
                                 .OrderByDescending(c => c.RentalRequests.Count);
                    break;
                case "notrented":
                    ViewBag.PageTitle = "Cars Not Rented Yet";
                    ViewBag.Heading = "Cars Not Rented Yet";
                    query = query.Where(c => c.RentalRequests.Count == 0);
                    break;
                default:
                    ViewBag.PageTitle = "All Cars";
                    ViewBag.Heading = "All Cars";
                    break;
            }

            var cars = await query.ToListAsync();

            return View(cars);
        }


        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.CarID == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarViewModel carViewModel)
        {
            if (ModelState.IsValid)
            {
                var account = new Account(
            "niwahang",
            "795422516494254",
            "ydyImVZdZAVjunXmkcaWPiNTcKA");
                var cloudinary = new Cloudinary(account);

                var uploadResult = new ImageUploadResult();
                if (carViewModel.CarImage != null)
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(carViewModel.CarImage.FileName, carViewModel.CarImage.OpenReadStream())
                    };
                    uploadResult = await cloudinary.UploadAsync(uploadParams);
                }

                var car = new Car
                {
                    Manufacturer = carViewModel.Manufacturer,
                    Model = carViewModel.Model,
                    RentalRate = carViewModel.RentalRate,
                    VehicleNo = carViewModel.VehicleNo,
                    IsAvailable = true,
                    CarImageURL = uploadResult.SecureUrl?.ToString()
                };

                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(carViewModel);
        }


        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarID,Manufacturer,Model,RentalRate,VehicleNo")] Car car)
        {
            if (id != car.CarID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.CarID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.CarID == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.CarID == id);
        }
    }
}
