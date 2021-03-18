using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Models
{
    public class CarBrand
    {
        [Key]
        public int CarBrandID { get; set; }
        public string Brand { get; set; }
    }
}
