﻿using Auctioneer.Data;
using Auctioneer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Auctioneer.ViewModels
{
    public class AuctionsViewModel
    {
        public List<AuctionViewModel> Auctions { get; set; }
    }

    public class Builder
    {
        public List<Auction> GetAllAuctions(ApplicationDbContext _db)
        {
            var result = _db.Auction.Include(a => a.CarBrand).Include(b => b.CarType).Include(c => c.Gallery).ToList();

            return result;
        }
        public Auction GetAuctionByID(ApplicationDbContext _db, int id)
        {
            List<Auction> auctions = _db.Auction.Include(a => a.CarBrand).Include(b => b.CarType).Include(c => c.Gallery).Include( d => d.AuctionCarFeatures).ToList();
            foreach (var auction in auctions)
            {
                if (auction.AuctionID == id)
                {
                    return auction;
                }
            }
            return null;
        }

       
        public async Task<BidViewModel> AuctionModelToBidsVMAsync(Auction auction, UserManager<IdentityUser> _userManager)
        {
            BidViewModel bidViewModel = new();
            bidViewModel.Image = auction.Gallery.FirstOrDefault();
            bidViewModel.AuctionID = auction.AuctionID;
            bidViewModel.Title = auction.Title;
            bidViewModel.Description = auction.Description;
            bidViewModel.CreationDate = auction.CreationDate;
            bidViewModel.Duration = auction.Duration;
            bidViewModel.MaxBid = auction.MaxBid;
            bidViewModel.MinBid = auction.MinBid;
            bidViewModel.CurrentBid = auction.CurrentBid;
            bidViewModel.Brand = auction.CarBrand.Brand;
            bidViewModel.Type = auction.CarType.Type;

            var user = await _userManager.FindByIdAsync(auction.AuctionOwnerID);
            bidViewModel.AuctionOwner = user.UserName;
            user = await _userManager.FindByIdAsync(auction.AuctionWinnerID);
            bidViewModel.AuctionWinner = user != null ? user.UserName : "None";
            return bidViewModel;
        }
        public Auction VMtoAuctionModel (AuctionViewModel auctionViewModel)
        {
            Auction auction = new();
            auction.Duration = (int)auctionViewModel.Duration;
            auction.Title = auctionViewModel.Title;
            auction.Description = auctionViewModel.Description;
            auction.MinBid = (int)auctionViewModel.MinBid;
            auction.MaxBid = (int)auctionViewModel.MaxBid;
            auction.CarBrandID = (int)auctionViewModel.CarBrandID;
            auction.CarTypeID = (int)auctionViewModel.CarTypeID;
            auction.CreationDate = DateTime.Now;
            auction.AuctionCarFeatures = new List<AuctionCarFeatures>();
            auction.Gallery = new List<Gallery>();

            return auction;
        }

        public int GetUserLastBid(ApplicationDbContext _db, string userID, int auctionID)
        {
            List<Bids> bids = _db.Bids.ToList();
            foreach (var bid in bids)
            {
                if (bid.UserID == userID && bid.AuctionID == auctionID)
                {
                    return bid.Amount;
                }
            }
            return 0;
        }

        public List<Bids> GetAllUserBids(ApplicationDbContext _db, string userID)
        {
            List<Bids> allUserBids = new();
            List<Bids> bids = _db.Bids.ToList();
            foreach (var bid in bids)
            {
                if (bid.UserID == userID)
                {
                    allUserBids.Add(bid);
                }
            }
            return allUserBids;
        }

        public List<Auction> GetAuctionsByOwnerID(ApplicationDbContext _db, string userID)
        {
            List<Auction> userAuctions = new();
            List<Auction> auctions = _db.Auction.Include(a => a.CarBrand).Include(b => b.CarType).Include(c => c.Gallery).ToList();
            foreach (var auction in auctions)
            {
                if (auction.AuctionOwnerID == userID)
                {
                    userAuctions.Add(auction);
                }
            }
            return userAuctions;
        }

        public string SaveImageToFile(IWebHostEnvironment _hostEnvironment, IFormFile imageFile)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
            string extension = Path.GetExtension(imageFile.FileName);
            fileName = fileName + DateTime.Now.ToString("ddmmyyyy") + extension;
            string path = Path.Combine(wwwRootPath + "/Image/", fileName);
            imageFile.CopyTo(new FileStream(path, FileMode.Create));
            return fileName;
        }
    }
}
