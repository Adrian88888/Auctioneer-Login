using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.ViewModels
{
    public class DepositsViewModel
    {
        public int DepositsID { get; set; }
        public int Amount { get; set; }
        public int Balance { get; set; }
        public string UserID { get; set; }
    }
}
