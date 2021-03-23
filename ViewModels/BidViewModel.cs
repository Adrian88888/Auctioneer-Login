using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.ViewModels
{
    public class BidViewModel
    {
        public int BidID { get; set; }
        public int Amount { get; set; }
        public string UserID { get; set; }
        public int AuctionID { get; set; }
        public int Min_bid { get; set; }
        public int Max_bid { get; set; }
        public int Current_bid { get; set; }
    }
}
