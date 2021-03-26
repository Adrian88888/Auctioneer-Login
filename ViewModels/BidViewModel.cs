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
    public class BidViewModel
    {
        public int BidID { get; set; }
        public int Amount { get; set; }
        public string UserID { get; set; }
        public int AuctionID { get; set; }
        public int MinBid { get; set; }
        public int MaxBid { get; set; }
        public int Balance { get; set; }
        public string AuctionOwner { get; set; }
        public string AuctionWinner { get; set; }
        public int Duration { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int UserLastBid { get; set; }
        public int CurrentBid { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate => CreationDate.AddDays(Duration);
        public string Brand { get; set; }
        public string  Type { get; set; }
        public Gallery Image { get; set; }
    }


}
