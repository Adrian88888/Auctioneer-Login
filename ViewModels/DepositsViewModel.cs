using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.ViewModels
{
    public class DepositsViewModel
    {
        public int DepositsID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int Amount { get; set; }
        public int Balance { get; set; }
        public string UserID { get; set; }
        public string StatusMessage { get; set; }
    }

}