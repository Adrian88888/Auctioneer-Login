using Auctioneer.Data;
using Auctioneer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Controllers
{
    public class CarBrandController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CarBrandController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var model = _db.CarBrand.ToList();
            return View(model);
        }
        [HttpGet]
        public IActionResult CreateBrand()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CarBrand carBrand)
        {
            _db.CarBrand.Add(carBrand);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var carBrand = _db.CarBrand.Where(x => x.CarBrandID == id).FirstOrDefault();
            _db.Remove(carBrand);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var carBrand = _db.CarBrand.Where(x => x.CarBrandID == id).FirstOrDefault();
            return View(carBrand);
        }
        [HttpPost]
        public IActionResult Edit(CarFeatures editedFeature)
        {
            var id = editedFeature.CarFeatureID;
            CarFeatures feature = _db.CarFeatures.Where(x => x.CarFeatureID == id).FirstOrDefault();
            feature.CarFeature = editedFeature.CarFeature;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
