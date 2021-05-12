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
using Database.Repository;
using Database.Models;
using Database.Data;
using Auctioneer.Services;
using Auctioneer.Services.BidsServices;

namespace Auctioneer.Controllers
{
    public class AuctionController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public AuctionController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager)
        {
            this._hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _db = db;
        }
        [AllowAnonymous]
        public async Task<IActionResult> IndexAsync()
        {
            AuctionsViewModel model = new();
            model.Auctions = new List<AuctionViewModel>();
            AuctionRepository auctionRepo = new(_db);
            var auctions = auctionRepo.GetAllAuctions();

            foreach (var auction in auctions)
            {
                if (auction.ExpiryDate > DateTime.Now && auction.IsBlocked != true)
                {
                    AuctionViewModel auctionViewModel = new();
                    await auctionViewModel.AuctionModelToVMAsync(auction, _userManager);
                    var userID = _userManager.GetUserId(User);
                    BidService bidService = new();
                    auctionViewModel.UserLastBid = bidService.GetUserLastBid(_db, userID, auction.AuctionID);
                    model.Auctions.Add(auctionViewModel);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> MyAuctionsAsync()
        {
            BidService bidService = new();
            AuctionRepository auctionRepo = new(_db);
            AuctionsViewModel model = new();
            model.Auctions = new List<AuctionViewModel>();
            var userID = _userManager.GetUserId(User);
            var userAuctions = auctionRepo.GetAuctionsByOwnerID(userID);
            foreach (var userAuction in userAuctions)
            {
                AuctionViewModel auctionViewModel = new();
                await auctionViewModel.AuctionModelToVMAsync(userAuction, _userManager);
                auctionViewModel.UserLastBid = bidService.GetUserLastBid(_db, userID, userAuction.AuctionID);
                model.Auctions.Add(auctionViewModel);
            }
            return View(model);
        }


        [AllowAnonymous]
        public async Task<IActionResult> ExpiredAuctionsAsync()
        {
            AuctionsViewModel model = new();
            model.Auctions = new List<AuctionViewModel>();

            AuctionRepository auctionRepo = new(_db);
            var auctions = auctionRepo.GetAllAuctions();

            BidService bidService = new();

            foreach (var auction in auctions)
            {
                if (auction.ExpiryDate < DateTime.Now && !auction.IsBlocked)
                {
                    AuctionViewModel auctionViewModel = new();
                    await auctionViewModel.AuctionModelToVMAsync(auction, _userManager);
                    //load user last bid
                    var userID = _userManager.GetUserId(User);
                    auctionViewModel.UserLastBid = bidService.GetUserLastBid(_db, userID, auction.AuctionID);
                    model.Auctions.Add(auctionViewModel);
                }
            }
            return View(model);
        }


        public IActionResult Create()
        {
            CarBrandRepository carBrandRepo = new(_db);
            CarFeaturesRepository carFeaturesRepo = new(_db);
            AuctionViewModel model = new()
            {
                Brands = carBrandRepo.GetAllCarBrands(),
                Features = carFeaturesRepo.GetAllCarFeatures()
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
                AuctionRepository auctionRepo = new(_db);
                auctionRepo.AddAuction(auction);
                return RedirectToAction("Details", new { id = auction.AuctionID });
            }
            CarBrandRepository carBrandRepo = new(_db);
            CarFeaturesRepository carFeaturesRepo = new(_db);
            auctionViewModel.Brands = carBrandRepo.GetAllCarBrands();
            auctionViewModel.Features = carFeaturesRepo.GetAllCarFeatures();
            return View(auctionViewModel);
        }
        public ActionResult GetTypesByBrand(int id)
        {
            CarTypeRepository carTypeRepo = new(_db);
            var types = carTypeRepo.GetTypesByBrand(id);
            return Json(types);
        }
        [AllowAnonymous]
        public async Task<IActionResult> DetailsAsync(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            AuctionRepository auctionRepo = new(_db);
            Auction auction = auctionRepo.GetAuctionByID((int)id);
            AuctionViewModel model = new();
            await model.AuctionModelToVMAsync(auction, _userManager);
            model.Features = new List<CarFeatures>();
            CarFeaturesRepository carFeaturesRepo = new(_db);
            foreach (var auctionCarFeature in model.AuctionCarFeatures)
            {
                CarFeatures feature = carFeaturesRepo.GetCarFeatureById(_db, auctionCarFeature.CarFeaturesID);
                if (feature != null)
                {
                    model.Features.Add(feature);
                }

            }
            BidService bidService = new();
            var userID = _userManager.GetUserId(User);
            model.UserLastBid = bidService.GetUserLastBid(_db, userID, (int)id);

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Block(int id)
        {
            var auction = _db.Auction.Where(a => a.AuctionID == id).FirstOrDefault();
            auction.IsBlocked = true;
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
