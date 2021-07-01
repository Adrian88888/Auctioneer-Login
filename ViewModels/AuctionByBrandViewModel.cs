using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.ViewModels
{
    public class AuctionByBrandViewModel
    {
        [DisplayName("Number of auctions")]
        public int Number { get; set; }
        public string Brand { get; set; }
    }
}
