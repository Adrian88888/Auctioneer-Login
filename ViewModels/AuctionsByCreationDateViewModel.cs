using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.ViewModels
{
    public class AuctionsByCreationDateViewModel
    {
        [DisplayName("Start date")]
        public DateTime StartDate { get; set; }
        [DisplayName("End date")]
        public DateTime EndDate { get; set; }
        [DisplayName("Auctions created")]
        public int AuctionCount { get; set; }
    }
}
