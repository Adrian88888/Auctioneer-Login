using Database.Models;
using System;
using System.Collections.Generic;

namespace Auctioneer.ViewModels
{
    public class AuctionByMonthViewModel
    {
        public List<AuctionByBrandViewModel> AuctionsByBrand { get; set; }
        public DateTime Period { get; set; }
    }
}