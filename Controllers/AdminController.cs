using Auctioneer.Data;
using Auctioneer.Models;
using Auctioneer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Controllers
{
    public class AdminController : Controller
    {
        public AdminController()
        {
        }
        [Authorize(Policy ="readpolicy")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
