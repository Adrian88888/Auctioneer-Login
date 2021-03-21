using Auctioneer.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Controllers
{
    public class BidsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BidsController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            this._hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
