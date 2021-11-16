using Auctioneer.ViewModels;
using Database.Models;
using Database.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Controllers
{
    public class AuctionController : Controller
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly ICarBrandRepository _carBrandRepository;
        private readonly ICarTypeRepository _carTypeRepository;
        private readonly ICarFeaturesRepository _carFeaturesRepository;
        private readonly BidService _bidService;
        private readonly DummyService _dummyService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(AuctionController));


        public AuctionController(DummyService dummyService, BidService bidService, IAuctionRepository auctionRepository, ICarTypeRepository carTypeRepository, ICarBrandRepository carBrandRepository, ICarFeaturesRepository carFeaturesRepository, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager)
        {
            this._hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _auctionRepository = auctionRepository;
            _carTypeRepository = carTypeRepository;
            _carBrandRepository = carBrandRepository;
            _carFeaturesRepository = carFeaturesRepository;
            _bidService = bidService;
            _dummyService = dummyService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> IndexAsync(string searchInput, int? carBrandID, string sort, string sortBy)
        {
            logger.Info("Accessing the Index action of the Auction Controller");
            AuctionsViewModel model = new();

            model.Brands = _carBrandRepository.GetAllCarBrands();
            model.Auctions = new List<AuctionViewModel>();

            List<Auction> auctions = new();

            if (!String.IsNullOrEmpty(searchInput))
            {
                auctions = _auctionRepository.GetLiveAuctionsByKeyword(searchInput);
                if (auctions.Count > 0)
                {
                    model.StatusMessage = "Showing the auctions that contain '" + searchInput + "' within the title or description.";
                }
                else
                {
                    model.StatusMessage = "There are no auctions that contain '" + searchInput + "' within the title or description.";
                }

            }
            else
            {
                auctions = _auctionRepository.GetLiveAuctions();
            }


            if (carBrandID != null)
            {
                var carBrand = _carBrandRepository.GetAllCarBrands().Where(a => a.CarBrandID == carBrandID).FirstOrDefault().Brand;
                auctions = auctions.Where(a => a.CarBrandID == carBrandID).ToList();
                if (auctions.Count > 0)
                {
                    model.StatusMessage = "Showing the live auctions for the brand " + carBrand + ".";
                }
                else
                {
                    model.StatusMessage = "There are no live auctions for the brand " + carBrand + " at the moment.";
                }

            }

            if (auctions != null)
            {
                if (sort == "asc")
                {
                    auctions = sortBy switch
                    {
                        "expiry" => auctions.OrderBy(a => a.ExpiryDate).ToList(),
                        "min_bid" => auctions.OrderBy(a => a.MinBid).ToList(),
                        "max_bid" => auctions.OrderBy(a => a.MaxBid).ToList(),
                        _ => auctions.OrderBy(a => a.CreationDate).ToList(),
                    };
                }
                else
                {
                    auctions = sortBy switch
                    {
                        "expiry" => auctions.OrderByDescending(a => a.ExpiryDate).ToList(),
                        "min_bid" => auctions.OrderByDescending(a => a.MinBid).ToList(),
                        "max_bid" => auctions.OrderByDescending(a => a.MaxBid).ToList(),
                        _ => auctions.OrderByDescending(a => a.CreationDate).ToList(),
                    };
                }

                foreach (var auction in auctions)
                {

                    AuctionViewModel auctionViewModel = new();
                    await auctionViewModel.AuctionModelToVMAsync(auction, _userManager);

                    if (auctionViewModel.Gallery.Count == 0)
                    {
                        Gallery dummyImage = new();
                        dummyImage.Image = _dummyService.GetDummy();
                        auctionViewModel.Gallery.Add(dummyImage);
                    }

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
            else
            {
                model.StatusMessage = "There are no active auctions at the moment. Please try again later.";
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

                if (auctionViewModel.Gallery.Count == 0)
                {
                    Gallery dummyImage = new();
                    dummyImage.Image = _dummyService.GetDummy();
                    auctionViewModel.Gallery.Add(dummyImage);
                }

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

                    if (auctionViewModel.Gallery.Count == 0)
                    {
                        Gallery dummyImage = new();
                        dummyImage.Image = _dummyService.GetDummy();
                        auctionViewModel.Gallery.Add(dummyImage);
                    }

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
        public async Task<IActionResult> CreateAsync(AuctionViewModel auctionViewModel)
        {
            if (ModelState.IsValid)
            {

                Auction auction = new();
                auctionViewModel.VMtoAuctionModel(auction);


                foreach (var image in auctionViewModel.ImageFiles)
                {
                    if (image.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await image.CopyToAsync(stream);
                            Gallery convertedImage = new();
                            convertedImage.Image = stream.ToArray();
                            auction.Gallery.Add(convertedImage);
                        }
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

            if (model.Gallery.Count == 0)
            {
                Gallery dummyImage = new();
                dummyImage.Image = _dummyService.GetDummy();
                model.Gallery.Add(dummyImage);
            }

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
