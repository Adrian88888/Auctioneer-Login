using Auctioneer.Data;
using Auctioneer.Models;
using Auctioneer.ViewModels;
using Microsoft.AspNetCore.Hosting;
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
            List<Bids> bids = _db.Bids.ToList();
            List<Auction> auctions = _db.Auction.Include(a => a.CarBrand).Include(b => b.CarType).Include(c => c.Gallery).ToList();
            AuctionsViewModel model = new()
            {
                Auctions = new List<AuctionViewModel>()
            };
            var userID = _userManager.GetUserId(User);


            foreach ( var bid in bids)
            { 
                if (bid.UserID == userID)
                { 
            foreach (var auction in auctions)
            {
                        if (auction.AuctionID == bid.AuctionID)
                        {
                            AuctionViewModel auctionViewModel = new();

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
                            List<Bids> userBids = _db.Bids.ToList();
                            foreach (var userBid in userBids)
                            {
                                if (userBid.UserID == userID && userBid.AuctionID == auction.AuctionID)
                                {
                                    auctionViewModel.UserLastBid = userBid.Amount;
                                }
                            }
                            model.Auctions.Add(auctionViewModel);
                        }
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> CreateAsync(int? id)
        {
            BidViewModel model = new();
            var userID = _userManager.GetUserId(User);

            if (_db.Deposits.Any(d => d.UserID == userID))
            {
                model.Balance = _db.Deposits.FirstOrDefault(x => x.UserID == userID).Balance;
            }
            else
            {
                model.Balance = 0;
            }

            List<Auction> auctions = _db.Auction.Include(a => a.CarBrand).Include(b => b.CarType).Include(c => c.Gallery).ToList();
            foreach (var auction in auctions)
            {
                if (auction.AuctionID == id)
                {
                    model.Image = auction.Gallery.FirstOrDefault();
                    model.MinBid = auction.MinBid;
                    model.MaxBid = auction.MaxBid;
                    model.CurrentBid = auction.CurrentBid;
                    model.AuctionID = auction.AuctionID;
                    model.Title = auction.Title;
                    model.Description = auction.Description;
                    model.CreationDate = auction.CreationDate;
                    model.Duration = auction.Duration;
                    model.Brand = auction.CarBrand.Brand;
                    model.Type = auction.CarType.Type;
                    var user = await _userManager.FindByIdAsync(auction.AuctionOwnerID);
                    model.AuctionOwner = user.UserName;
                    if (auction.AuctionWinnerID == "None")
                    {
                        model.AuctionWinner = "None";
                    }
                    else
                    {
                        user = await _userManager.FindByIdAsync(auction.AuctionWinnerID);
                        model.AuctionWinner = user.UserName;
                    }

                    Bids userBid = _db.Bids.Where(b => b.UserID == userID).Where(b => b.AuctionID == auction.AuctionID).FirstOrDefault();
                    
                    if (userBid == null)

                    {
                        model.UserLastBid = 0;
                    }
                    else
                    {
                        model.UserLastBid = userBid.Amount;
                    }

                    return View(model);
                }
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Create(BidViewModel model, int? id)
        {
            if (ModelState.IsValid)
            {
                var userID = _userManager.GetUserId(User);
                Auction auction = _db.Auction.FirstOrDefault(x => x.AuctionID == (int)id);
                Deposits deposit = _db.Deposits.FirstOrDefault(x => x.UserID == userID);

                //check that the bided amount is higher than the min bid/current bid and lower than his current balance
                if (model.Amount > auction.CurrentBid && model.Amount > auction.MinBid && model.Amount <= model.Balance)
                {
                    //check if the user has bided for the same auction before and modify the previous bid amount if he did.
                    Boolean existing = false;
                    List<Bids> existingBids = _db.Bids.ToList();
                    foreach (var existingBid in existingBids)
                    {
                        if (existingBid.UserID == userID && existingBid.AuctionID == id)
                        {
                            existingBid.Amount = model.Amount;
                            existing = true;
                        }
                    }
                    auction.CurrentBid = model.Amount;
                    auction.AuctionWinnerID = userID;
                    //if the user never bided for the same auction before, add a new bid
                    if (!existing)
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
                        ModelState.AddModelError("Amount", "The bid amount must be higher than the minimum bid/current bid.");
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
