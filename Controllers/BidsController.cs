using Database.Data;
using Database.Models;
using Auctioneer.Services;
using Auctioneer.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Repository;
using Auctioneer.Services.BidsServices;
using System;

namespace Auctioneer.Controllers
{
    public class BidsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public BidsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            AuctionsViewModel model = new()
            {
                Auctions = new List<AuctionViewModel>()
            };

            var userID = _userManager.GetUserId(User);
            BidService bidsService = new();
            var allUserBids = bidsService.GetAllUserBids(_db, userID);
            AuctionRepository auctionRepo = new(_db);
            var auctions = auctionRepo.GetAllAuctions();
            foreach (var bid in allUserBids)
            {
                foreach (var auction in auctions)
                {
                    if (auction.AuctionID == bid.AuctionID)
                    {
                        AuctionViewModel auctionViewModel = new();
                        await auctionViewModel.AuctionModelToVMAsync(auction, _userManager);
                        auctionViewModel.UserLastBid = bidsService.GetUserLastBid(_db, userID, auction.AuctionID);
                        model.Auctions.Add(auctionViewModel);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> CreateAsync(int? id)
        {
            AuctionRepository auctionRepo = new(_db);
            Auction auction = auctionRepo.GetAuctionByID((int)id);
            BidViewModel model = new();
            await model.AuctionModelToBidsVMAsync(auction, _userManager);
            BalanceService balanceService = new(_db);
            var userID = _userManager.GetUserId(User);
            model.Balance = balanceService.GetUserBalance(userID);
            BidService bidService = new();
            model.UserLastBid = bidService.GetUserLastBid(_db, userID, (int)id);

            return View(model);
        }
        [HttpPost]
        public IActionResult Create(BidViewModel model, int? id)
        {
            if (ModelState.IsValid)
            {
                var userID = _userManager.GetUserId(User);
                AuctionRepository auctionRepo = new(_db);
                Auction auction = auctionRepo.GetAuctionByID((int)id);
                //if user bid more or equal to the winning bid the auction should end 
                if (model.Amount >= auction.MaxBid && model.Amount <= model.Balance)
                {
                    var userBids = _db.Bids.Where(x => x.UserID == userID).ToList();
                    var userBid = userBids.FirstOrDefault(x => x.AuctionID == id);
                    if (userBid != null)
                    {
                        userBid.Amount = model.Amount;
                        auction.CurrentBid = model.Amount;
                        auction.AuctionWinnerID = userID;

                    }
                    else
                    {
                        Bids bid = new();
                        auction.CurrentBid = model.Amount;
                        auction.AuctionWinnerID = userID;
                        bid.Amount = model.Amount;
                        bid.UserID = userID;
                        bid.AuctionID = (int)id;
                        _db.Bids.Add(bid);
                    }
                    auction.ExpiryDate = DateTime.Now;
                    _db.SaveChanges();
                    return RedirectToAction("WinningBid");
                }
                //check that the bided amount is higher than the min bid/current bid and lower than his current balance
                if (model.Amount > auction.CurrentBid && model.Amount > auction.MinBid && model.Amount <= model.Balance)
                {
                    var userBids = _db.Bids.Where(x => x.UserID == userID).ToList();
                    var userBid = userBids.FirstOrDefault(x => x.AuctionID == id);
                    if (userBid != null)
                    {
                        userBid.Amount = model.Amount;
                        auction.CurrentBid = model.Amount;
                        auction.AuctionWinnerID = userID;

                    }
                    else
                    {
                        Bids bid = new();
                        auction.CurrentBid = model.Amount;
                        auction.AuctionWinnerID = userID;
                        bid.Amount = model.Amount;
                        bid.UserID = userID;
                        bid.AuctionID = (int)id;
                        _db.Bids.Add(bid);
                    }
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                //if the bided amount is not valid, return view with model error
                else
                {
                    if (model.Amount <= auction.CurrentBid || model.Amount <= auction.MinBid)
                    {
                        model.Gallery = auction.Gallery;
                        ModelState.AddModelError("Amount", "Your bid must be higher than the minimum bid/current bid.");
                        return View(model);
                    }
                    else
                    {
                        model.Gallery = auction.Gallery;
                        ModelState.AddModelError("Amount", "You cannot bid more than you account balance!");
                        return View(model);
                    }
                }
            }
            return View(model);
        }
        public IActionResult WinningBid()
        {
            return View();
        }
    }
}
