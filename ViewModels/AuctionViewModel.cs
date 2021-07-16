using Database.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.ViewModels
{
    public class AuctionViewModel
    {

        
        public int AuctionID { get; set; }
        public string AuctionOwner { get; set; }
        public string AuctionWinner { get; set; }
        public int UserLastBid { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        public int? Duration { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [DisplayName("Starting Bid")]
        [Required(ErrorMessage = "Starting bid is required")]
        public int? MinBid { get; set; }
        [DisplayName("Winning Bid")]
        [Required(ErrorMessage = "Winning bid is required")]
        public int? MaxBid { get; set; }
        public int? CurrentBid { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        [DisplayName("Upload Image")]
        [Required(ErrorMessage = "Car image is required")]
        public List<IFormFile> ImageFiles { get; set; }
        [DisplayName("Car Brand")]
        [Required(ErrorMessage = "Car brand is required")]
        public int? CarBrandID { get; set; }
        public string Brand { get; set; }

        public List<CarBrand> Brands { get; set; }
        [DisplayName("Car Model")]
        [Required(ErrorMessage = "Car model is required")]
        public int? CarTypeID { get; set; }
        public string Type { get; set; }
        public List<Gallery> Gallery { get; set; }
        public List<CarFeatures> Features { get; set; }
        public List<AuctionCarFeatures> AuctionCarFeatures { get; internal set; }


        public async Task AuctionModelToVMAsync(Auction auction, UserManager<IdentityUser> _userManager)
        { 
            if(auction.Gallery.Count > 0)
            {
                Gallery = auction.Gallery;
            }
            else
            {
                //Gallery dummy = new();
                //dummy.ImageName = "dummy.jpg";
                //List<Gallery> dummyGallery = new();
                //dummyGallery.Add(dummy);
                //Gallery = dummyGallery;
            }
            AuctionID = auction.AuctionID;
            Title = auction.Title;
            Description = auction.Description;
            CreationDate = auction.CreationDate;
            ExpiryDate = auction.ExpiryDate;
            Duration = auction.Duration;
            MaxBid = auction.MaxBid;
            MinBid = auction.MinBid;
            CurrentBid = auction.CurrentBid;
            Brand = auction.CarBrand.Brand;
            Type = auction.CarType.Type;
            AuctionCarFeatures = auction.AuctionCarFeatures;
            var user = await _userManager.FindByIdAsync(auction.AuctionOwnerID);
            if (user != null)
            {
                AuctionOwner = user.UserName;
            }
            else
            {
                auction.IsBlocked = true;
            }
           
            user = await _userManager.FindByIdAsync(auction.AuctionWinnerID);
            AuctionWinner = user != null ? user.UserName : "None";
        }
        public void VMtoAuctionModel(Auction auction)
        {
            auction.Duration = (int)Duration;
            auction.Title = Title;
            auction.Description = Description;
            auction.MinBid = (int)MinBid;
            auction.MaxBid = (int)MaxBid;
            auction.CarBrandID = (int)CarBrandID;
            auction.CarTypeID = (int)CarTypeID;
            auction.CreationDate = DateTime.Now;
            auction.ExpiryDate = DateTime.Now.AddDays(auction.Duration);
            auction.AuctionCarFeatures = new List<AuctionCarFeatures>();
            auction.Gallery = new List<Gallery>();
        }
    }
}
