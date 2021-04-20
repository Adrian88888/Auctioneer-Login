using Auctioneer.Data;
using Auctioneer.Models;
using Auctioneer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> IndexAsync()
        {
            AuctionsViewModel model = new();
            model.Auctions = new List<AuctionViewModel>();

            Builder builder = new();
            List<Auction> auctions = builder.GetAllAuctions(_db);

            foreach (var auction in auctions)
            {
                if (auction.ExpiryDate > DateTime.Now)
                {
                    AuctionViewModel auctionViewModel = new();
                     await auctionViewModel.AuctionModelToVMAsync(auction, _userManager);
                    var userID = _userManager.GetUserId(User);
                    auctionViewModel.UserLastBid = builder.GetUserLastBid(_db, userID, auction.AuctionID);
                    model.Auctions.Add(auctionViewModel);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> MyAuctionsAsync()
        {
            Builder builder = new();
            AuctionsViewModel model = new();
            model.Auctions = new List<AuctionViewModel>();
            var userID = _userManager.GetUserId(User);
            var userAuctions = builder.GetAuctionsByOwnerID(_db, userID);
            foreach (var userAuction in userAuctions)
            {
                AuctionViewModel auctionViewModel = new();
                await auctionViewModel.AuctionModelToVMAsync(userAuction, _userManager);
                auctionViewModel.UserLastBid = builder.GetUserLastBid(_db, userID, userAuction.AuctionID);
                model.Auctions.Add(auctionViewModel);
            }
            return View(model);
        }


        [AllowAnonymous]
        public async Task<IActionResult> ExpiredAuctionsAsync()
        {
            AuctionsViewModel model = new();
            model.Auctions = new List<AuctionViewModel>();

            Builder builder = new();
            var auctions = builder.GetAllAuctions(_db);

            foreach (var auction in auctions)
            {
                if (auction.ExpiryDate < DateTime.Now)
                {
                    AuctionViewModel auctionViewModel = new();
                    await auctionViewModel.AuctionModelToVMAsync(auction, _userManager);
                    //load user last bid
                    var userID = _userManager.GetUserId(User);
                    auctionViewModel.UserLastBid = builder.GetUserLastBid(_db, userID, auction.AuctionID);
                    model.Auctions.Add(auctionViewModel);
                }
            }
            return View(model);
        }


        public IActionResult Create()
        {
            AuctionViewModel model = new()
            {
                Brands = _db.CarBrand.ToList(),
                Features = _db.CarFeatures.ToList()
            };

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AuctionViewModel auctionViewModel)
        {
            if (ModelState.IsValid)
            {
                Builder builder = new();
                Auction auction = builder.VMtoAuctionModel(auctionViewModel);

                if (auctionViewModel.ImageFiles != null && auctionViewModel.ImageFiles.Count > 0)
                {
                    foreach (IFormFile imageFile in auctionViewModel.ImageFiles)
                    {
                        var gallery = new Gallery
                        {
                            ImageName = builder.SaveImageToFile(_hostEnvironment, imageFile)
                        };
                        auction.Gallery.Add(gallery);
                    }
                }
                if (auctionViewModel.Features != null && auctionViewModel.Features.Count > 0)
                {
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
                auction.AuctionOwnerID = _userManager.GetUserId(User);
                _db.Auction.Add(auction);
                _db.SaveChanges();
                return RedirectToAction("Details", new { id = auction.AuctionID });
            }
            auctionViewModel.Brands = _db.CarBrand.ToList();
            auctionViewModel.Features = _db.CarFeatures.ToList();
            return View(auctionViewModel);
        }
        public ActionResult GetTypesByBrand(int id)
        {
            var types = _db.CarType.Where(x => x.CarBrandID == id).Select(x => new { id = x.CarTypeID, type = x.Type }).ToList();
            return Json(types);
        }
        [AllowAnonymous]
        public async Task<IActionResult> DetailsAsync(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Builder builder = new();
            Auction auction = builder.GetAuctionByID(_db, (int)id);
            AuctionViewModel model = new();
            await model.AuctionModelToVMAsync(auction, _userManager);
            model.Features = new List<CarFeatures>();
            foreach (var  auctionCarFeature in model.AuctionCarFeatures)
            {
                CarFeatures feature = _db.CarFeatures.Where(a => a.CarFeatureID == auctionCarFeature.CarFeaturesID).FirstOrDefault();
                if (feature != null)
                {
                    model.Features.Add(feature);
                }
                
            }
            var userID = _userManager.GetUserId(User);
            model.UserLastBid = builder.GetUserLastBid(_db, userID, (int)id);

            return View(model);
        }
    }
}
