using Auctioneer.Data;
using Auctioneer.Models;
using Auctioneer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Controllers
{
    public class DepositsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DepositsController(ApplicationDbContext db)
        {
            _db = db;
        }



        public IActionResult AddFunds()
        {
            DepositsViewModel model = new();

            if (_db.Deposits.Any(d => d.UserID == User.Identity.Name))
            {
                model.Balance = _db.Deposits.FirstOrDefault(x => x.UserID == User.Identity.Name).Balance;
            }
            else
            {
                model.Balance = 0;
            }
            return View(model);
        }





        [HttpPost]
        public IActionResult AddFunds(DepositsViewModel model)
        {
            if (_db.Deposits.Any(d => d.UserID == User.Identity.Name))
            {
                Deposits balance = _db.Deposits.Where(b => b.UserID == User.Identity.Name).FirstOrDefault();
                balance.Balance += model.Amount;
            }
            else
            {
                Deposits newDeposit = new();
                newDeposit.Balance = model.Amount;
                newDeposit.UserID = User.Identity.Name;
                _db.Deposits.Add(newDeposit);
            }
            _db.SaveChanges();

            if (_db.Deposits.Any(d => d.UserID == User.Identity.Name))
            {
                model.Balance = _db.Deposits.FirstOrDefault(x => x.UserID == User.Identity.Name).Balance;
            }
            else
            {
                model.Balance = 0;
            }
            return View(model);
        }
    }
}
