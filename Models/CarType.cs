using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Models
{
    public class CarType
    {
        [Key]
        public int CarTypeID { get; set; }
        [DisplayName("Car Model")]
        public string Type { get; set; }
        public int CarBrandID { get; set; }
    }
}
