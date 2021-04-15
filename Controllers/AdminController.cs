using Auctioneer.Data;
using Auctioneer.Models;
using Auctioneer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Authorize(Policy ="readpolicy")]
        public IActionResult Index()
        {
            AdminViewModel model = new();
            model.CarBrands = _db.CarBrand.ToList();
            model.CarFeatures = _db.CarFeatures.ToList();

            return View(model);
        }
        [Authorize(Policy = "readpolicy")]
        public IActionResult CarFeatures()
        {
            var model = _db.CarFeatures.ToList();
            return View(model);
        }
        [HttpGet]
        [Authorize(Policy = "writepolicy")]
        public IActionResult CreateFeature()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateFeature(CarFeatures feature)
        {
            _db.CarFeatures.Add(feature);
            _db.SaveChanges();
            return RedirectToAction("CarFeatures");
        }
        public IActionResult DeleteFeature(int id)
        {
            var feature = _db.CarFeatures.Where(x => x.CarFeatureID == id).FirstOrDefault();
            _db.Remove(feature);
            _db.SaveChanges();
            return RedirectToAction("CarFeatures");
        }
        [HttpGet]
        public IActionResult EditFeature(int id)
        {
            var feature = _db.CarFeatures.Where(x => x.CarFeatureID == id).FirstOrDefault();
            return View(feature);
        }
        [HttpPost]
        public IActionResult EditFeature(CarFeatures editedFeature)
        {
            var id = editedFeature.CarFeatureID;
            CarFeatures feature = _db.CarFeatures.Where(x => x.CarFeatureID == id).FirstOrDefault();
            feature.CarFeature = editedFeature.CarFeature;
            _db.SaveChanges();
            return RedirectToAction("CarFeatures");
        }
        public IActionResult CarBrands()
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
        public IActionResult CreateBrand(CarBrand carBrand)
        {
            _db.CarBrand.Add(carBrand);
            _db.SaveChanges();
            return RedirectToAction("CarBrands");
        }
        public IActionResult DeleteBrand(int id)
        {
            var carBrand = _db.CarBrand.Where(x => x.CarBrandID == id).FirstOrDefault();
            _db.Remove(carBrand);
            _db.SaveChanges();
            return RedirectToAction("CarBrands");
        }
        [HttpGet]
        public IActionResult EditBrand(int id)
        {
            var carBrand = _db.CarBrand.Where(x => x.CarBrandID == id).FirstOrDefault();
            return View(carBrand);
        }
        [HttpPost]
        public IActionResult EditBrand(CarFeatures editedFeature)
        {
            var id = editedFeature.CarFeatureID;
            CarFeatures feature = _db.CarFeatures.Where(x => x.CarFeatureID == id).FirstOrDefault();
            feature.CarFeature = editedFeature.CarFeature;
            _db.SaveChanges();
            return RedirectToAction("CarBrands");
        }
    }
}
