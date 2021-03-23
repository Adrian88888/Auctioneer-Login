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
            //List<Bids> bids = _db.Bids.ToList();
            //List<Bids> model = new();

            //foreach (var bid in bids)
            //{
            //    if (bid.UserID == User.Identity.Name)
            //    {
            //        model.Add(bid);
            //    }
            //}
            //    return View(model);
            List<Bids> bids = _db.Bids.ToList();
            List<Bids> highestBids = new();
            List<Auction> auctions = _db.Auction.Include(a => a.CarBrand).Include(b => b.CarType).Include(c => c.Gallery).ToList();
            AuctionsViewModel model = new()
            {
                Auctions = new List<AuctionViewModel>()
            };
            foreach (var highBid in bids)
            {

            }

            foreach ( var bid in bids)
            { 
                if (bid.UserID == User.Identity.Name)
                { 
            foreach (var auction in auctions)
            {
                        if (auction.AuctionID == bid.AuctionID)
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
            Auction auction = _db.Auction.FirstOrDefault(x => x.AuctionID == (int)id);
            model.Min_bid = auction.Min_bid;
            model.Max_bid = auction.Max_bid;
            model.Current_bid = auction.Current_bid;

            return View(model);
        }
        [HttpPost]
        public IActionResult Create(BidViewModel model, int? id)
        {
            if (ModelState.IsValid)
            {
                Auction auction = _db.Auction.FirstOrDefault(x => x.AuctionID == (int)id);
                //check that the bided amount is higher than the min bid/current bid
                if (model.Amount > auction.Current_bid && model.Amount > auction.Min_bid)
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
                    auction.Current_bid = model.Amount;
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
                    model.Min_bid = auction.Min_bid;
                    model.Max_bid = auction.Max_bid;
                    model.Current_bid = auction.Current_bid;
                    ModelState.AddModelError("Amount", "The bid amount must be higher than the minimum bid/current bid.");
                    return View(model);
                }
            }
            return View(model);
        }
    }
}
