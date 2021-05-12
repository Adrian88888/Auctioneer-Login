using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.ViewModels
{
    public class AdminViewModel
    {
        public List<CarFeatures> CarFeatures { get; set; }
        public List<CarBrand> CarBrands { get; set; }
    }
}
