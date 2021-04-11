using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DisplayName("Car Feature")]
        public string CarFeature { get; set; }

        public bool IsSelected { get; set; }
    }
}
