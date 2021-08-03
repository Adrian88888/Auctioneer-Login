using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.ViewModels
{
    public class DummyImageViewModel
    {
        public IFormFile Image { get; set; }
        public byte[] DummyImage { get; set; }
    }
}
