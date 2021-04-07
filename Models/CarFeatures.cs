using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Models
{
    public class CarFeatures
    {
        [Key]
        public int CarFeatureID { get; set; }
        public string CarFeature { get; set; }

        public bool IsSelected { get; set; }
    }
}
