using Auctioneer.ViewModels;
using Database.Data;
using Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Controllers
{

    public class DummyImageController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DummyImageController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            Dummy dummy = _db.Dummy.FirstOrDefault();
            DummyImageViewModel model = new();

            if (dummy != null)
            {
                model.DummyImage = dummy.Image;
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync(DummyImageViewModel dummy)
        {
            Dummy dummyImage = new();

            if (dummy.Image.Length > 0)
            {
               

                using (var stream = new MemoryStream())
                {
                    await dummy.Image.CopyToAsync(stream);
                    dummyImage.Image = stream.ToArray();
                }
            }
            Dummy dummyGallery = _db.Dummy.FirstOrDefault();

            if (dummyGallery != null)
            {
                dummyGallery.Image = dummyImage.Image;
            }
            else
            {
                _db.Dummy.Add(dummyImage);
            }
           
            _db.SaveChanges();

            DummyImageViewModel model = new();
            model.DummyImage = dummyImage.Image;

            return View(model);
        }
    }
}
