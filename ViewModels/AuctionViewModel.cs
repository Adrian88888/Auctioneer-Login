using Auctioneer.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.ViewModels
{
    public class AuctionViewModel
    {

        
        public int AuctionID { get; set; }

        public int Duration { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [DisplayName("Starting Bid")]
        public int Min_bid { get; set; }
        [DisplayName("Winning Bid")]
        public int Max_bid { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate
        {
            get { return CreationDate.AddDays(Duration); }
        }

        public string ImageName { get; set; }
        [DisplayName("Upload Image")]
        public List<IFormFile> ImageFiles { get; set; }
        [DisplayName("Car Brand")]
        public int CarBrandID { get; set; }
        public string Brand { get; set; }

        public List<CarBrandViewModel> Brands { get; set; }
        [DisplayName("Car Model")]
        public int CarTypeID { get; set; }
        public string Type { get; set; }
        public List<CarTypeViewModel> Types { get; set; }
        public Gallery Image { get; set; }
        public List<Gallery> Images { get; set; }
        
    }
}
