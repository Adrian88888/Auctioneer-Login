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
        public BidsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Bids> bids = _db.Bids.ToList();
            List<Auction> auctions = _db.Auction.Include(a => a.CarBrand).Include(b => b.CarType).Include(c => c.Gallery).ToList();
            AuctionsViewModel model = new()
            {
                Auctions = new List<AuctionViewModel>()
            };
            foreach ( var bid in bids)
            { 
                if (bid.UserID == User.Identity.Name)
                { 
            foreach (var auction in auctions)
            {
                        if (auction.AuctionID == bid.AuctionID)
                        {
                            AuctionViewModel auctionViewModel = new();

                            auctionViewModel.Image = auction.Gallery.FirstOrDefault();
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
                            auctionViewModel.AuctionOwner = auction.AuctionOwner;
                            auctionViewModel.AuctionWinner = auction.AuctionWinner;
                            //load user last bid
                            List<Bids> userBids = _db.Bids.ToList();
                            foreach (var userBid in userBids)
                            {
                                if (userBid.UserID == User.Identity.Name && userBid.AuctionID == auction.AuctionID)
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
        public IActionResult Create(int? id)
        {
            BidViewModel model = new();           

            if (_db.Deposits.Any(d => d.UserID == User.Identity.Name))
            {
                model.Balance = _db.Deposits.FirstOrDefault(x => x.UserID == User.Identity.Name).Balance;
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
                    model.AuctionOwner = auction.AuctionOwner;
                    model.AuctionWinner = auction.AuctionWinner;

                    if (_db.Bids.Any(d => d.UserID == User.Identity.Name) && _db.Bids.Any(b => b.AuctionID == auction.AuctionID))
                    {
                        Bids userBid = _db.Bids.Where(b => b.UserID == User.Identity.Name).Where(b => b.AuctionID == auction.AuctionID).FirstOrDefault();
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
                Auction auction = _db.Auction.FirstOrDefault(x => x.AuctionID == (int)id);
                Deposits deposit = _db.Deposits.FirstOrDefault(x => x.UserID == User.Identity.Name);

                //check that the bided amount is higher than the min bid/current bid and lower than his current balance
                if (model.Amount > auction.CurrentBid && model.Amount > auction.MinBid && model.Amount <= model.Balance)
                {
                    //check if the user has bided for the same auction before and modify the previous bid amount if he did.
                    Boolean existing = false;
                    List<Bids> existingBids = _db.Bids.ToList();
                    foreach (var existingBid in existingBids)
                    {
                        if (existingBid.UserID == User.Identity.Name && existingBid.AuctionID == id)
                        {
                            existingBid.Amount = model.Amount;
                            existing = true;
                        }
                    }
                    auction.CurrentBid = model.Amount;
                    auction.AuctionWinner = User.Identity.Name;
                    //if the user never bided for the same auction before, add a new bid
                    if (!existing)
                    {
                        Bids bid = new();
                        bid.Amount = model.Amount;
                        bid.UserID = User.Identity.Name;
                        bid.AuctionID = (int)id;
                        _db.Bids.Add(bid);
                    }
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                //if the bided amount is not valid, return view with model error
                else
                {
                    if (model.Amount < auction.CurrentBid || model.Amount < auction.MinBid)
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
