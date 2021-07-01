using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;

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
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            logger.Error($"The path {exceptionDetails.Path} threw an exception: {exceptionDetails.Error}");

            return View();
        }
    }
}
