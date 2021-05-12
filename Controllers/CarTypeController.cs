using Database.Data;
using Database.Models;
using Auctioneer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Auctioneer.Controllers
{
    public class CarTypeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CarTypeController(ApplicationDbContext db)
        {
            _db = db;
        }
        [Authorize(Policy = "readpolicy")]
        public IActionResult Index()
        {
            var model = _db.CarBrand.ToList();
            return View(model);
        }
        public IActionResult EditBrandModels(int id)
        {
            ViewBag.Id = id;
            EditBrandModelsViewModel model = new();

            model.CarTypes = _db.CarType.Where(a => a.CarBrandID == id).ToList();
            model.Brand = _db.CarBrand.FirstOrDefault(a => a.CarBrandID == id).Brand.ToString();
            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            CarType model = _db.CarType.Where(a => a.CarTypeID == id).FirstOrDefault();
            return View(model);
        }
        public IActionResult Edit(CarType editedType)
        {
            if (ModelState.IsValid)
            {
                int id = editedType.CarTypeID;
                CarType carType = _db.CarType.Where(x => x.CarTypeID == id).FirstOrDefault();
                carType.Type = editedType.Type;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Create(CarType input, int id)
        {
            if (ModelState.IsValid)
            {
                CarType carType = new();
                carType.CarBrandID = id;
                carType.Type = input.Type;
                _db.CarType.Add(carType);
                _db.SaveChanges();

                return RedirectToAction("EditBrandModels", new { id });
            }
            return View();
        }
        public IActionResult Delete(int id)
        {
            var carType = _db.CarType.Where(x => x.CarTypeID == id).FirstOrDefault();
            _db.CarType.Remove(carType);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}