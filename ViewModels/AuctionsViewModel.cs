using Auctioneer.Data;
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

        public async Task<AuctionViewModel> AuctionModelToVMAsync(Auction auction, UserManager<IdentityUser> _userManager)
        {
            AuctionViewModel auctionViewModel = new();
            auctionViewModel.Images = new List<Gallery>();

            //load auction images from Gallery 
            List<Gallery> images = auction.Gallery;

            foreach (var image in images)
            {
                auctionViewModel.Images.Add(image);
                auctionViewModel.Image = image;
            }
            //load auction details from database
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
            user = await _userManager.FindByIdAsync(auction.AuctionWinnerID);
            if (user != null)
            {
                auctionViewModel.AuctionWinner = user.UserName;
            }
            else
            {
                auctionViewModel.AuctionWinner = "None";
            }



            return auctionViewModel;
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
