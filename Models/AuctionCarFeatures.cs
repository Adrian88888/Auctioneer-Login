using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Models
{
    public class AuctionCarFeatures
    {
        [Key]
        public int AuctionCarFeaturesID { get; set; }
        public int CarFeaturesID { get; set; }
    }
}
