using Auctioneer.Data;
using Auctioneer.Models;
using Auctioneer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;

        public AuctionController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager)
        {
            _db = db;
            this._hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<IActionResult> IndexAsync()
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
                auctionViewModel.MaxBid = auction.MaxBid;
                auctionViewModel.MinBid = auction.MinBid;
                auctionViewModel.CurrentBid = auction.CurrentBid;
                auctionViewModel.Brand = auction.CarBrand.Brand;
                auctionViewModel.Type = auction.CarType.Type;

                var user = await _userManager.FindByIdAsync(auction.AuctionOwnerID);
                auctionViewModel.AuctionOwner = user.UserName;
                if (auction.AuctionWinnerID == "None")
                {
                    auctionViewModel.AuctionWinner = "None";
                }
                else
                {
                    user = await _userManager.FindByIdAsync(auction.AuctionWinnerID);
                    auctionViewModel.AuctionWinner = user.UserName;
                }
                //load user last bid
                List<Bids> bids = _db.Bids.ToList();

                var userID = _userManager.GetUserId(User);

                foreach (var bid in bids)
                {
                    if (bid.UserID == userID && bid.AuctionID == auction.AuctionID)
                    {
                        auctionViewModel.UserLastBid = bid.Amount;
                    }
                }

                model.Auctions.Add(auctionViewModel);
            }
            return View(model);
        }





        public async System.Threading.Tasks.Task<IActionResult> MyAuctionsAsync()
        {
            var userID = _userManager.GetUserId(User);
            List<Auction> auctions = _db.Auction.Include(a => a.CarBrand).Include(b => b.CarType).Include(c => c.Gallery).ToList();
            AuctionsViewModel model = new()
            {
                Auctions = new List<AuctionViewModel>()
            };


            foreach (var auction in auctions)
            {
                if (auction.AuctionOwnerID == userID)
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
                auctionViewModel.MaxBid = auction.MaxBid;
                auctionViewModel.MinBid = auction.MinBid;
                auctionViewModel.CurrentBid = auction.CurrentBid;
                auctionViewModel.Brand = auction.CarBrand.Brand;
                auctionViewModel.Type = auction.CarType.Type;
                    var user = await _userManager.FindByIdAsync(auction.AuctionOwnerID);
                 auctionViewModel.AuctionOwner = user.UserName;
                    if (auction.AuctionWinnerID == "None")
                    {
                        auctionViewModel.AuctionWinner = "None";
                    }
                    else
                    {
                        user = await _userManager.FindByIdAsync(auction.AuctionWinnerID);
                        auctionViewModel.AuctionWinner = user.UserName;
                    }

                    model.Auctions.Add(auctionViewModel);
            }
            }
            return View(model);
        }





        [AllowAnonymous]
        public async System.Threading.Tasks.Task<IActionResult> ExpiredAuctionsAsync()
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
                auctionViewModel.MaxBid = auction.MaxBid;
                auctionViewModel.MinBid = auction.MinBid;
                auctionViewModel.Brand = auction.CarBrand.Brand;
                auctionViewModel.Type = auction.CarType.Type;
                auctionViewModel.Title = auction.Title;
                var user = await _userManager.FindByIdAsync(auction.AuctionOwnerID);
                auctionViewModel.AuctionOwner = user.UserName;
                user = await _userManager.FindByIdAsync(auction.AuctionWinnerID);
                if (auction.AuctionWinnerID == "None")
                {
                    auctionViewModel.AuctionWinner = "None";
                }
                else
                {
                    user = await _userManager.FindByIdAsync(auction.AuctionWinnerID);
                    auctionViewModel.AuctionWinner = user.UserName;
                }

                model.Auctions.Add(auctionViewModel);
            }
            return View(model);
        }





        public IActionResult Create()
        {
           List<CarBrand> carBrands = _db.CarBrand.ToList();
           List<CarType> carTypes = _db.CarType.ToList();
           List<CarFeatures> carFeatures = _db.CarFeatures.ToList();

            AuctionViewModel model = new()
            {
                Brands = new List<CarBrandViewModel>(),
                Types = new List<CarTypeViewModel>(),
                Features = new List<CarFeatures>()
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
            foreach (var carFeature in carFeatures)
            {
                model.Features.Add(carFeature);
            }

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AuctionViewModel auctionViewModel)
        {
            if (ModelState.IsValid)
            {
                var auction = new Auction();
                //save image to wwwroot/image
                if ( auctionViewModel.ImageFiles != null && auctionViewModel.ImageFiles.Count > 0)
                {
                    auction.Gallery = new List<Gallery>();
                    
                    foreach (IFormFile imageFile in auctionViewModel.ImageFiles )
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
                //save car features
                if (auctionViewModel.Features != null && auctionViewModel.Features.Count > 0)
                {
                    auction.AuctionCarFeatures = new List<AuctionCarFeatures>();

                    foreach (var feature in auctionViewModel.Features)
                    {
                        if (feature.IsSelected)
                        {
                            var auctionCarFeature = new AuctionCarFeatures
                            {
                                CarFeaturesID = feature.CarFeatureID

                            };
                            auction.AuctionCarFeatures.Add(auctionCarFeature);
                        }
                    }
                }
                //insert the record to database

                auction.Duration = (int)auctionViewModel.Duration;
                auction.Title = auctionViewModel.Title;
                auction.Description = auctionViewModel.Description;
                auction.MinBid = (int)auctionViewModel.MinBid;
                auction.MaxBid = (int)auctionViewModel.MaxBid;
                auction.CarBrandID = (int)auctionViewModel.CarBrandID;
                auction.CarTypeID = (int)auctionViewModel.CarTypeID;
                auction.CreationDate = DateTime.Now;
                auction.AuctionOwnerID = _userManager.GetUserId(User);
                auction.AuctionWinnerID = "None";


                _db.Auction.Add(auction);
                _db.SaveChanges();
                return RedirectToAction("Display", new { id = auction.AuctionID });
            }
            List<CarBrand> carBrands = _db.CarBrand.ToList();
            auctionViewModel.Brands = new List<CarBrandViewModel>();

            foreach (var carBrand in carBrands)
            {
                var carBrandViewModel = new CarBrandViewModel
                {
                    CarBrandID = carBrand.CarBrandID,
                    Brand = carBrand.Brand
                };
                auctionViewModel.Brands.Add(carBrandViewModel);
            }
            List<CarFeatures> carFeatures = _db.CarFeatures.ToList();
            auctionViewModel.Features = new List<CarFeatures>();

            foreach (var carFeature in carFeatures)
            {
                auctionViewModel.Features.Add(carFeature);
            }

            return View(auctionViewModel);
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

            var auctions = _db.Auction.Include(a => a.CarBrand).Include(b => b.CarType).Include(c => c.Gallery).Include(d => d.AuctionCarFeatures);
            var model = new AuctionViewModel();

            foreach (var auction in auctions)
            {
                if (auction.AuctionID == id)
                {
                    model.AuctionID = auction.AuctionID;
                    model.Duration = auction.Duration;
                    model.Description = auction.Description;
                    model.CreationDate = auction.CreationDate;
                    model.MinBid = auction.MinBid;
                    model.MaxBid = auction.MaxBid;
                    model.CurrentBid = auction.CurrentBid;
                    model.Brand = auction.CarBrand.Brand;
                    model.Type = auction.CarType.Type;
                    model.Images = auction.Gallery;
                    model.Title = auction.Title;
                    model.Features = new();

                    List<AuctionCarFeatures> auctionCarFeatures = auction.AuctionCarFeatures;

                    foreach (var auctionCarFeature in auctionCarFeatures)
                    {
                        var feature = _db.CarFeatures.Where(x => x.CarFeatureID == auctionCarFeature.CarFeaturesID).FirstOrDefault();
                        model.Features.Add(feature);

                    }

                    List<Bids> bids = _db.Bids.ToList();
                    var userID = _userManager.GetUserId(User);
                    foreach (var bid in bids)
                    {
                        if (bid.UserID == userID && bid.AuctionID == auction.AuctionID)
                        {
                            model.UserLastBid = bid.Amount;
                        }
                    }
                }
            }
            return View(model);
        }
    }
}
