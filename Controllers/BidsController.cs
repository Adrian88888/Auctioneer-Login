﻿using Database.Data;
using Database.Models;
using Services;
using Auctioneer.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Repository;
using System;

namespace Auctioneer.Controllers
{
    public class BidsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly BidService _bidService;
        private readonly BalanceService _balanceService;
        private readonly IAuctionRepository _auctionRepository;
        private readonly IBidRepository _bidRepository;


        public BidsController(UserManager<IdentityUser> userManager, BalanceService balanceService, BidService bidService, IAuctionRepository auctionRepository, IBidRepository bidRepository)
        {
            _userManager = userManager;
            _balanceService = balanceService;
            _bidService = bidService;
            _auctionRepository = auctionRepository;
            _bidRepository = bidRepository;
        }



        public async Task<IActionResult> IndexAsync()
        {
            AuctionsViewModel model = new()
            {
                Auctions = new List<AuctionViewModel>()
            };

            var userID = _userManager.GetUserId(User);

            var allUserBids = _bidService.GetAllUserBids(userID);

            var auctions = _auctionRepository.GetAllAuctions();
            foreach (var bid in allUserBids)
            {
                foreach (var auction in auctions)
                {
                    if (auction.AuctionID == bid.AuctionID)
                    {
                        AuctionViewModel auctionViewModel = new();
                        await auctionViewModel.AuctionModelToVMAsync(auction, _userManager);
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
            }
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> CreateAsync(int? id)
        { 
            Auction auction = _auctionRepository.GetAuction((int)id);
            BidViewModel model = new();

            await model.AuctionModelToBidsVMAsync(auction, _userManager);

            var userID = _userManager.GetUserId(User);
            model.Balance = _balanceService.GetUserBalance(userID);

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



        [HttpPost]
        public IActionResult Create(BidViewModel model, int? id)
        {
            if (ModelState.IsValid)
            {
                var userID = _userManager.GetUserId(User);
                Auction auction = _auctionRepository.GetAuction((int)id);
                //if user bid more or equal to the winning bid the auction should end 
                if (model.Amount >= auction.MaxBid && model.Amount <= model.Balance)
                {
                    var userLastBid = _bidService.GetUserLastBid(userID, auction.AuctionID);
                    if (userLastBid != null)
                    {
                        userLastBid.Amount = model.Amount;
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
                        _bidRepository.Add(bid);
                    }
                    auction.ExpiryDate = DateTime.Now;
                    _bidRepository.Save();

                    return RedirectToAction("WinningBid");
                }
                //check that the bided amount is higher than the min bid/current bid and lower than his current balance
                if (model.Amount > auction.CurrentBid && model.Amount > auction.MinBid && model.Amount <= model.Balance)
                {
                    var userLastBid = _bidService.GetUserLastBid(userID, auction.AuctionID);
                    if (userLastBid != null)
                    {
                        userLastBid.Amount = model.Amount;
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
                        _bidRepository.Add(bid);
                    }
                    _bidRepository.Save(); 

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
