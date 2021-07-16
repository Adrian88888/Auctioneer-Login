using Auctioneer.ViewModels;
using Database.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Controllers
{
    public class ReportsController : Controller
    {
        public IAuctionRepository _auctionRepository;
        public ICarBrandRepository _carBrandRepository;

        public ReportsController(IAuctionRepository auctionRepository, ICarBrandRepository carBrandRepository)
        {
            _auctionRepository = auctionRepository;
            _carBrandRepository = carBrandRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AuctionsByBrand()
        {
            List<AuctionByBrandViewModel> model = new();
            var brands = _carBrandRepository.GetAllCarBrands();

            foreach (var brand in brands)
            {
                AuctionByBrandViewModel auctionByBrandViewModel = new();
                auctionByBrandViewModel.Brand = brand.Brand;
                auctionByBrandViewModel.Number = _auctionRepository.GetAuctionsByBrandID(brand.CarBrandID).Count();

                model.Add(auctionByBrandViewModel);
            }
            return View(model);
        }
        public IActionResult AuctionsByCreationDate(DateTime startDate, DateTime endDate)
        {
            AuctionsByCreationDateViewModel model = new();
            model.StartDate = startDate.Date;
            model.EndDate = endDate.Date;
            model.AuctionCount = _auctionRepository.GetAuctionsByCreationDate(model.StartDate.Date, model.EndDate.Date).Count();


            return View(model);
        }
        public IActionResult AuctionsByMonth(DateTime startDate)
        {
            var firstDayOfMonth = new DateTime(startDate.Year, startDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            AuctionByMonthViewModel model = new();
            model.AuctionsByBrand = new List<AuctionByBrandViewModel>();
            model.SelectedMonth = startDate;

            var brands = _carBrandRepository.GetAllCarBrands();

            foreach (var brand in brands)
            {
                AuctionByBrandViewModel auctionByBrandViewModel = new();
                auctionByBrandViewModel.Brand = brand.Brand;
                auctionByBrandViewModel.Number = _auctionRepository.GetAuctionsByBrandIDandCreationDate(brand.CarBrandID, firstDayOfMonth, lastDayOfMonth).Count;

                model.AuctionsByBrand.Add(auctionByBrandViewModel);
            }
            return View(model);
        }

        public IActionResult AuctionsByYear(DateTime startYear)
        {
            AuctionsByYearViewModel model = new();
            model.AuctionsByYear = new();
            DateTime endDate = DateTime.Now;
            DateTime iterator = new DateTime(startYear.Year, 1, 1);

            while (iterator < endDate)
            {
                var firstDayOfMonth = new DateTime(iterator.Year, iterator.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                AuctionByMonthViewModel month = new();
                month.AuctionsByBrand = new List<AuctionByBrandViewModel>();
                month.SelectedMonth = iterator;

                var brands = _carBrandRepository.GetAllCarBrands();

                foreach (var brand in brands)
                {
                    AuctionByBrandViewModel auctionByBrandViewModel = new();
                    auctionByBrandViewModel.Brand = brand.Brand;
                    auctionByBrandViewModel.Number = _auctionRepository.GetAuctionsByBrandIDandCreationDate(brand.CarBrandID, firstDayOfMonth, lastDayOfMonth).Count;

                    month.AuctionsByBrand.Add(auctionByBrandViewModel);
                }
                model.AuctionsByYear.Add(month);
                iterator = iterator.AddMonths(1);
            }
            return View(model);
        }
    }
}
