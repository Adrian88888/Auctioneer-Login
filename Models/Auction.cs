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
        [NotMapped]
        [DisplayName("Upload Images:")]
        public List<IFormFile> ImageFiles { get; set; }
        [DisplayName("Car Brand")]
        public int CarBrandID { get; set; }
        public virtual CarBrand CarBrand { get; set; }
        public int CarTypeID { get; set; }
        public virtual CarType CarType { get; set; }
        public int GalleryID { get; set; }
        public virtual List<Gallery> Gallery { get; set; }
    }

}
