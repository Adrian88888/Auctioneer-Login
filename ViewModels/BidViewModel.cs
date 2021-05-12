using Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.ViewModels
{
    public class BidViewModel : AuctionViewModel
    {
        public int BidID { get; set; }
        public int Amount { get; set; }
        public int Balance { get; set; }
        public Gallery Image { get; set; }
        public new int? CarTypeID { get; set; }
        public new int? CarBrandID { get; set; }
        public new List<IFormFile> ImageFiles { get; set; }
        public async Task AuctionModelToBidsVMAsync(Auction auction, UserManager<IdentityUser> _userManager)
        {
            Gallery = auction.Gallery;
            AuctionID = auction.AuctionID;
            Title = auction.Title;
            Description = auction.Description;
            CreationDate = auction.CreationDate;
            Duration = auction.Duration;
            MaxBid = auction.MaxBid;
            MinBid = auction.MinBid;
            CurrentBid = auction.CurrentBid;
            Brand = auction.CarBrand.Brand;
            Type = auction.CarType.Type;

            var user = await _userManager.FindByIdAsync(auction.AuctionOwnerID);
            AuctionOwner = user.UserName;
            user = await _userManager.FindByIdAsync(auction.AuctionWinnerID);
            AuctionWinner = user != null ? user.UserName : "None";
        }
    }


}
