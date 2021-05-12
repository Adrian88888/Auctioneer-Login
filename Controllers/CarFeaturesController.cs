using Database.Data;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Controllers
{
    public class CarFeaturesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CarFeaturesController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var model = _db.CarFeatures.ToList();
            return View(model);
        }
        [HttpGet]
        [Authorize(Policy = "writepolicy")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CarFeatures feature)
        {
            _db.CarFeatures.Add(feature);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var feature = _db.CarFeatures.Where(x => x.CarFeatureID == id).FirstOrDefault();
            _db.Remove(feature);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var feature = _db.CarFeatures.Where(x => x.CarFeatureID == id).FirstOrDefault();
            return View(feature);
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
