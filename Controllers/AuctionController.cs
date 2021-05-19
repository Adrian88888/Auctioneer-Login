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

namespace Auctioneer.Controllers
{
    public class AuctionController : Controller
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly ICarBrandRepository _carBrandRepository;
        private readonly ICarTypeRepository _carTypeRepository;
        private readonly ICarFeaturesRepository _carFeaturesRepository;
        private readonly BidService _bidService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(AuctionController));


        public AuctionController(BidService bidService, IAuctionRepository auctionRepository, ICarTypeRepository carTypeRepository, ICarBrandRepository carBrandRepository, ICarFeaturesRepository carFeaturesRepository, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager)
        {
            this._hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _auctionRepository = auctionRepository;
            _carTypeRepository = carTypeRepository;
            _carBrandRepository = carBrandRepository;
            _carFeaturesRepository = carFeaturesRepository;
            _bidService = bidService;
        }



        [AllowAnonymous]
        public async Task<IActionResult> IndexAsync()
        {
            logger.Info("Accessing the Index action of the Auction Controller");
            AuctionsViewModel model = new();
            model.Auctions = new List<AuctionViewModel>();

            var auctions = _auctionRepository.GetLiveAuctions();

            foreach (var auction in auctions)
            {
                if (!auction.IsBlocked)
                {
                    AuctionViewModel auctionViewModel = new();
                    await auctionViewModel.AuctionModelToVMAsync(auction, _userManager);

                    var userID = _userManager.GetUserId(User);
                    var userLastBid = _bidService.GetUserLastBid(userID, auction.AuctionID);
                    if (userLastBid != null)
                    {
                        auctionViewModel.UserLastBid = userLastBid.Amount;
                    }
                    else
                    {
                        auctionViewModel.UserLastBid = 0;
                    }

                    model.Auctions.Add(auctionViewModel);
                }
            }
            return View(model);
        }



        public async Task<IActionResult> MyAuctionsAsync()
        {
            logger.Info("Accessing MyAuctions");

            AuctionsViewModel model = new();
            model.Auctions = new List<AuctionViewModel>();
            var userID = _userManager.GetUserId(User);
            var userAuctions = _auctionRepository.GetAuctionsByOwnerID(userID);

            foreach (var userAuction in userAuctions)
            {
                AuctionViewModel auctionViewModel = new();
                await auctionViewModel.AuctionModelToVMAsync(userAuction, _userManager);
                var userLastBid = _bidService.GetUserLastBid(userID, userAuction.AuctionID);
                if (userLastBid != null)
                {
                    auctionViewModel.UserLastBid = userLastBid.Amount;
                }
                else
                {
                    auctionViewModel.UserLastBid = 0;
                }
                
                model.Auctions.Add(auctionViewModel);
            }

            return View(model);
        }



        [AllowAnonymous]
        public async Task<IActionResult> ExpiredAuctionsAsync()
        {
            logger.Info("Accessing ExpiredAuctions");
            AuctionsViewModel model = new();
            model.Auctions = new List<AuctionViewModel>();

            var auctions = _auctionRepository.GetExpiredAuctions();

            foreach (var auction in auctions)
            {
                if (!auction.IsBlocked)
                {
                    AuctionViewModel auctionViewModel = new();
                    await auctionViewModel.AuctionModelToVMAsync(auction, _userManager);
                    //load user last bid
                    var userID = _userManager.GetUserId(User);
                    var userLastBid = _bidService.GetUserLastBid(userID, auction.AuctionID);
                    if (userLastBid != null)
                    {
                        auctionViewModel.UserLastBid = userLastBid.Amount;
                    }
                    else
                    {
                        auctionViewModel.UserLastBid = 0;
                    }

                    model.Auctions.Add(auctionViewModel);
                }
            }

            return View(model);
        }



        public IActionResult Create()
        {
            logger.Info("Accessing Create Auction");

            AuctionViewModel model = new()
            {
                Brands = _carBrandRepository.GetAllCarBrands(),
                Features = _carFeaturesRepository.GetAllCarFeatures()
            };

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AuctionViewModel auctionViewModel)
        {
            if (ModelState.IsValid)
            {

                Auction auction = new();
                auctionViewModel.VMtoAuctionModel(auction);

                if (auctionViewModel.ImageFiles != null && auctionViewModel.ImageFiles.Count > 0)
                {
                    foreach (IFormFile imageFile in auctionViewModel.ImageFiles)
                    {
                        var gallery = new Gallery
                        {
                            ImageName = auctionViewModel.SaveImageToFile(_hostEnvironment, imageFile)
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

                _auctionRepository.Add(auction);

                return RedirectToAction("Details", new { id = auction.AuctionID });
            }

            auctionViewModel.Brands = _carBrandRepository.GetAllCarBrands();
            auctionViewModel.Features = _carFeaturesRepository.GetAllCarFeatures();

            return View(auctionViewModel);
        }



        public ActionResult GetTypesByBrand(int id)
        {
            logger.Info($"Getting car types by brand id: {id}");

            var types = _carTypeRepository.GetTypesByBrand(id);

            return Json(types);
        }



        [AllowAnonymous]
        public async Task<IActionResult> DetailsAsync(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Auction auction = _auctionRepository.GetAuction((int)id);
            AuctionViewModel model = new();
            await model.AuctionModelToVMAsync(auction, _userManager);
            model.Features = new List<CarFeatures>();

            foreach (var auctionCarFeature in model.AuctionCarFeatures)
            {
                CarFeatures feature = _carFeaturesRepository.GetCarFeatureById(auctionCarFeature.CarFeaturesID);
                if (feature != null)
                {
                    model.Features.Add(feature);
                }

            }

            var userID = _userManager.GetUserId(User);
            var userLastBid = _bidService.GetUserLastBid(userID, (int)id);
            if (userLastBid != null)
            {
                model.UserLastBid = userLastBid.Amount;
            }
            else
            {
                model.UserLastBid = 0;
            }

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Block(int id, string returnUrl)
        {
            var auction = _auctionRepository.GetAuction(id); 
            auction.IsBlocked = true;
            _auctionRepository.Save();

            return Redirect(returnUrl);
        }
    }
}
