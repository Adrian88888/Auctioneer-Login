using Auctioneer.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        public int Duration { get; set; }

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
        public DateTime ExpiryDate
        {
            get
            {
                return CreationDate.AddDays((double)Duration);
            }
        }

        public string ImageName { get; set; }
        [DisplayName("Upload Image")]
        [Required(ErrorMessage = "Car image is required")]
        public List<IFormFile> ImageFiles { get; set; }
        [DisplayName("Car Brand")]
        [Required(ErrorMessage = "Car brand is required")]
        public int? CarBrandID { get; set; }
        public string Brand { get; set; }

        public List<CarBrandViewModel> Brands { get; set; }
        [DisplayName("Car Model")]
        [Required(ErrorMessage = "Car model is required")]
        public int? CarTypeID { get; set; }
        public string Type { get; set; }
        public List<CarTypeViewModel> Types { get; set; }
        public Gallery Image { get; set; }
        public List<Gallery> Images { get; set; }
        
    }
}
