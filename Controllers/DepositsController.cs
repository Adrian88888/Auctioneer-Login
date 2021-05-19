using Database.Data;
using Database.Models;
using Auctioneer.ViewModels;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;
        public DepositsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }



        public IActionResult AddFunds()
        {
            DepositsViewModel model = new();
            var userID = _userManager.GetUserId(User);

            if (_db.Deposits.Any(d => d.UserID == userID))
            {
                model.Balance = _db.Deposits.FirstOrDefault(x => x.UserID == userID).Balance;
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
            var userID = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {


                if (_db.Deposits.Any(d => d.UserID == userID))
                {
                    Deposits balance = _db.Deposits.Where(b => b.UserID == userID).FirstOrDefault();
                    balance.Balance += model.Amount;
                }
                else
                {
                    Deposits newDeposit = new();
                    newDeposit.Balance = model.Amount;
                    newDeposit.UserID = userID;
                    _db.Deposits.Add(newDeposit);
                }
                _db.SaveChanges();

                if (_db.Deposits.Any(d => d.UserID == userID))
                {
                    model.Balance = _db.Deposits.FirstOrDefault(x => x.UserID == userID).Balance;
                }
                else
                {
                    model.Balance = 0;
                }
                model.StatusMessage = $"You have added to your account $ {model.Amount} successfully!";
                return View(model);
            }

            if (_db.Deposits.Any(d => d.UserID == userID))
            {
                model.Balance = _db.Deposits.FirstOrDefault(x => x.UserID == userID).Balance;
            }
            else
            {
                model.Balance = 0;
            }
            return View(model);
        }
    }
}

