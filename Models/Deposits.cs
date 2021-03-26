using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Models
{
    public class Deposits
    {
        [Key]
        public int DepositsID { get; set; }
        public int Balance { get; set; }
        public string UserID { get; set; }
    }
}
