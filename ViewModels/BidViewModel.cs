using Auctioneer.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.ViewModels
{
    public class BidViewModel : AuctionViewModel
    {
        public int BidID { get; set; }
        public int Amount { get; set; }
        public int Balance { get; set; }
        public Gallery Image { get; set; }
        public new int? CarTypeID { get; set; }
        public new int? CarBrandID { get; set; }
        public new List<IFormFile> ImageFiles { get; set; }
    }


}
