using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Models
{
    public class Gallery
    {
        [Key]
        public int GalleryID { get; set; }
        public string ImageName { get; set; }
    }
}
