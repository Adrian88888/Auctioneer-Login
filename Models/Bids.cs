using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Models
{
    public class Bids
    {
        [Key]
        public int BidID { get; set; }
        [Required]
        public int Amount { get; set; }
        public  string UserID { get; set; }
        public int AuctionID { get; set; }
    }
}
