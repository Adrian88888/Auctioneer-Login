using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using Database.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {

        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(HomeController));

        [AllowAnonymous]
        public IActionResult Index()
        {
            logger.Info("Accessing the Index action of the Home Controller");
            return View();
        }
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
