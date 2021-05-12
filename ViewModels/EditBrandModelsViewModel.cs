using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.ViewModels
{
    public class EditBrandModelsViewModel
    {
        public List<CarType> CarTypes { get; set; }
        public string Brand { get; set; }
    }
}
