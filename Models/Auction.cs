using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Models
{
    public class Auction
    {
        public Auction()
        {

        }
        [Key]
        public int AuctionID { get; set; }
        public string AuctionOwnerID { get; set; }
        public string AuctionWinnerID { get; set; }
        public int Duration { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [DisplayName("Starting Bid")]
        public int MinBid { get; set; }
        [DisplayName("Winning Bid")]
        public int MaxBid { get; set; }
        public int CurrentBid { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate => CreationDate.AddDays(Duration);
        [NotMapped]
        [DisplayName("Upload Images:")]
        public List<IFormFile> ImageFiles { get; set; }
        [DisplayName("Car Brand")]
        public int CarBrandID { get; set; }
        public virtual CarBrand CarBrand { get; set; }
        public int CarTypeID { get; set; }
        public virtual CarType CarType { get; set; }
        public virtual List<Gallery> Gallery { get; set; }
        public virtual List<AuctionCarFeatures> AuctionCarFeatures { get; set; }
        public Boolean IsBlocked { get; set; }
    }

}
