using Auctioneer.Data;
using Auctioneer.Models;
using Auctioneer.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            Builder builder = new();
            var userID = _userManager.GetUserId(User);
            var allUserBids = builder.GetAllUserBids(_db, userID);
            List<Auction> auctions = builder.GetAllAuctions(_db);
            foreach (var bid in allUserBids)
            {
                foreach (var auction in auctions)
                {
                    if (auction.AuctionID == bid.AuctionID)
                    {
                        AuctionViewModel auctionViewModel = await builder.AuctionModelToVMAsync(auction, _userManager);
                        auctionViewModel.UserLastBid = builder.GetUserLastBid(_db, userID, auction.AuctionID);
                        model.Auctions.Add(auctionViewModel);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> CreateAsync(int? id)
        {
            var userID = _userManager.GetUserId(User);
            Builder builder = new();
            Auction auction = builder.GetAuctionByID(_db, (int)id);
            BidViewModel model = await builder.AuctionModelToBidsVMAsync(auction, _userManager);
            BalanceService balanceService = new(_db);
            model.Balance = balanceService.GetUserBalance(userID);
            model.UserLastBid = builder.GetUserLastBid(_db, userID, (int)id);

            return View(model);
        }
        [HttpPost]
        public IActionResult Create(BidViewModel model, int? id)
        {
            if (ModelState.IsValid)
            {
                var userID = _userManager.GetUserId(User);
                Builder builder = new();
                Auction auction = builder.GetAuctionByID(_db, (int)id);
                //check that the bided amount is higher than the min bid/current bid and lower than his current balance
                if (model.Amount > auction.CurrentBid && model.Amount > auction.MinBid && model.Amount <= model.Balance)
                {
                    var userBids = _db.Bids.Where(x => x.UserID == userID).ToList();
                    var userBid = userBids.FirstOrDefault(x => x.AuctionID == id);
                    if (userBid != null)
                    {
                        userBid.Amount = model.Amount;

                    }
                    else
                    {
                        Bids bid = new();
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
                        ModelState.AddModelError("Amount", "Your bid must be higher than the minimum bid/current bid.");
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError("Amount", "You cannot bid more than you account balance!");
                        return View(model);
                    }
                }
            }
            return View(model);
        }
    }
}
