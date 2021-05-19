using Database.Data;
using Database.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.ViewModels
{
    public class AuctionsViewModel
    {
        public List<AuctionViewModel> Auctions { get; set; }
    }

    public class Builder
    {









        public string SaveImageToFile(IWebHostEnvironment _hostEnvironment, IFormFile imageFile)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
            string extension = Path.GetExtension(imageFile.FileName);
            fileName = fileName + DateTime.Now.ToString("ddmmyyyy") + extension;
            string path = Path.Combine(wwwRootPath + "/Image/", fileName);
            imageFile.CopyTo(new FileStream(path, FileMode.Create));
            return fileName;
        }
    }
}
