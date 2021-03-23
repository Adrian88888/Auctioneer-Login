using Auctioneer.Data;
using Auctioneer.Models;
using Auctioneer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Auctioneer.Controllers
{
    public class AuctionController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AuctionController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            this._hostEnvironment = hostEnvironment;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            List<Auction>  auctions = _db.Auction.Include(a => a.CarBrand).Include(b => b.CarType).Include(c => c.Gallery).ToList();
            AuctionsViewModel model = new()
            {
                Auctions = new List<AuctionViewModel>()
            };


            foreach ( var auction in auctions )
            {
                var auctionViewModel = new AuctionViewModel
                {
                    Images = new List<Gallery>()
                };

                //load auction images from Gallery 
                List<Gallery> images = auction.Gallery;

                foreach ( var image in images )
                {
                    auctionViewModel.Images.Add(image);
                    auctionViewModel.Image = image;
                }
                //load auction details from database
                auctionViewModel.AuctionID = auction.AuctionID;
                auctionViewModel.Title = auction.Title;
                auctionViewModel.Description = auction.Description;
                auctionViewModel.CreationDate = auction.CreationDate;
                auctionViewModel.Duration = auction.Duration;
                auctionViewModel.Max_bid = auction.Max_bid;
                auctionViewModel.Min_bid = auction.Min_bid;
                auctionViewModel.Current_bid = auction.Current_bid;
                auctionViewModel.Brand = auction.CarBrand.Brand;
                auctionViewModel.Type = auction.CarType.Type;
                auctionViewModel.AuctionOwner = auction.AuctionOwner;
                auctionViewModel.AuctionWinner = auction.AuctionWinner;

                //load user last bid
                List<Bids> bids = _db.Bids.ToList();
                foreach (var bid in bids)
                {
                    if (bid.UserID == User.Identity.Name && bid.AuctionID == auction.AuctionID)
                    {
                        auctionViewModel.UserLastBid = bid.Amount;
                    }
                }

                model.Auctions.Add(auctionViewModel);
            }
            return View(model);
        }
        public IActionResult MyAuctions()
        {
            List<Auction> auctions = _db.Auction.Include(a => a.CarBrand).Include(b => b.CarType).Include(c => c.Gallery).ToList();
            AuctionsViewModel model = new()
            {
                Auctions = new List<AuctionViewModel>()
            };


            foreach (var auction in auctions)
            {
                if (auction.AuctionOwner == User.Identity.Name)
                { 
                    var auctionViewModel = new AuctionViewModel
                    {
                        Images = new List<Gallery>()
                    };
                List<Gallery> images = auction.Gallery;

                foreach (var image in images)
                {
                    auctionViewModel.Images.Add(image);
                    auctionViewModel.Image = image;
                }
                auctionViewModel.AuctionID = auction.AuctionID;
                auctionViewModel.Title = auction.Title;
                auctionViewModel.Description = auction.Description;
                auctionViewModel.CreationDate = auction.CreationDate;
                auctionViewModel.Duration = auction.Duration;
                auctionViewModel.Max_bid = auction.Max_bid;
                auctionViewModel.Min_bid = auction.Min_bid;
                auctionViewModel.Current_bid = auction.Current_bid;
                auctionViewModel.Brand = auction.CarBrand.Brand;
                auctionViewModel.Type = auction.CarType.Type;
                auctionViewModel.AuctionOwner = auction.AuctionOwner;
                auctionViewModel.AuctionWinner = auction.AuctionWinner;
                model.Auctions.Add(auctionViewModel);
            }
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ExpiredAuctions()
        {
            List<Auction> auctions = _db.Auction.Include(a => a.CarBrand).Include(b => b.CarType).Include(c => c.Gallery).ToList();
            AuctionsViewModel model = new()
            {
                Auctions = new List<AuctionViewModel>()
            };


            foreach (var auction in auctions)
            {
                var auctionViewModel = new AuctionViewModel
                {
                    Images = new List<Gallery>()
                };
                List<Gallery> images = auction.Gallery;

                foreach (var image in images)
                {
                    auctionViewModel.Images.Add(image);
                    auctionViewModel.Image = image;
                }
                auctionViewModel.AuctionID = auction.AuctionID;
                auctionViewModel.CreationDate = auction.CreationDate;
                auctionViewModel.Duration = auction.Duration;
                auctionViewModel.Max_bid = auction.Max_bid;
                auctionViewModel.Min_bid = auction.Min_bid;
                auctionViewModel.Brand = auction.CarBrand.Brand;
                auctionViewModel.Type = auction.CarType.Type;
                auctionViewModel.Title = auction.Title;
                auctionViewModel.AuctionOwner = auction.AuctionOwner;
                auctionViewModel.AuctionWinner = auction.AuctionWinner;
                model.Auctions.Add(auctionViewModel);
            }
            return View(model);
        }
        public IActionResult Create()
        {
           List<CarBrand> carBrands = _db.CarBrand.ToList();
           List<CarType> carTypes = _db.CarType.ToList();
            AuctionViewModel model = new()
            {
                Brands = new List<CarBrandViewModel>(),
                Types = new List<CarTypeViewModel>()
            };

            foreach ( var carBrand in carBrands)
            {
                var carBrandViewModel = new CarBrandViewModel
                {
                    CarBrandID = carBrand.CarBrandID,
                    Brand = carBrand.Brand
                };
                model.Brands.Add(carBrandViewModel);            
            }
            foreach (var carType in carTypes)
            {
                var carTypeViewModel = new CarTypeViewModel
                {
                    CarTypeID = carType.CarTypeID,
                    Type = carType.Type,
                    CarBrandID = carType.CarBrandID
                };
                model.Types.Add(carTypeViewModel);
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AuctionViewModel obj)
        {
            if (ModelState.IsValid)
            {
                var auction = new Auction();
                //save image to wwwroot/image
                if ( obj.ImageFiles != null && obj.ImageFiles.Count > 0)
                {
                    auction.Gallery = new List<Gallery>();
                    
                    foreach (IFormFile imageFile in obj.ImageFiles )
                    {
                        var gallery = new Gallery();
                       
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                string extension = Path.GetExtension(imageFile.FileName);
                gallery.ImageName = fileName = fileName + DateTime.Now.ToString("ddmmyyyy") + extension;
                string path = Path.Combine(wwwRootPath + "/Image/", fileName);
                 imageFile.CopyTo(new FileStream(path, FileMode.Create));
                        auction.Gallery.Add(gallery);
                    }
                }
                //insert the record to database
                
                auction.Duration = (int)obj.Duration;
                auction.Title = obj.Title;
                auction.Description = obj.Description;
                auction.Min_bid = (int)obj.Min_bid;
                auction.Max_bid = (int)obj.Max_bid;
                auction.CarBrandID = (int)obj.CarBrandID;
                auction.CarTypeID = (int)obj.CarTypeID;
                auction.CreationDate = DateTime.Now;
                auction.AuctionOwner = User.Identity.Name;
                auction.AuctionWinner = "None";
                _db.Auction.Add(auction);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<CarBrand> carBrands = _db.CarBrand.ToList();
            List<CarType> carTypes = _db.CarType.ToList();
            obj.Brands = new List<CarBrandViewModel>();
            obj.Types = new List<CarTypeViewModel>();

            foreach (var carBrand in carBrands)
            {
                var carBrandViewModel = new CarBrandViewModel
                {
                    CarBrandID = carBrand.CarBrandID,
                    Brand = carBrand.Brand
                };
                obj.Brands.Add(carBrandViewModel);
            }
            foreach (var carType in carTypes)
            {
                var carTypeViewModel = new CarTypeViewModel
                {
                    CarTypeID = carType.CarTypeID,
                    Type = carType.Type,
                    CarBrandID = carType.CarBrandID
                };
                obj.Types.Add(carTypeViewModel);
            }
           return View(obj);
        }
        public ActionResult GetTypesByBrand(int id)
        {
            var types = _db.CarType.Where(x => x.CarBrandID == id).Select(x => new { id = x.CarTypeID, type = x.Type }).ToList();
            return Json(types);
        }
        [AllowAnonymous]
        public IActionResult Display(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var auctions = _db.Auction.Include(a => a.CarBrand).Include(b => b.CarType).Include(c => c.Gallery);
            var model = new AuctionViewModel();

            foreach (var auction in auctions)
            {
                if (auction.AuctionID == id)
                {
                    model.Duration = auction.Duration;
                    model.Description = auction.Description;
                    model.CreationDate = auction.CreationDate;
                    model.Min_bid = auction.Min_bid;
                    model.Max_bid = auction.Max_bid;
                    model.Brand = auction.CarBrand.Brand;
                    model.Type = auction.CarType.Type;
                    model.Images = new List<Gallery>();
                    List<Gallery> images = auction.Gallery;

                    foreach (var image in images)
                    {
                        model.Images.Add(image);
                        model.Image = image;
                    }
                }
            }
            return View(model);
        }
    }
}
