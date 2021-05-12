using Database.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext _db;
        public RolesController(ApplicationDbContext _db, RoleManager<IdentityRole> roleManager)
        {
            this._db = _db;
            this.roleManager = roleManager;
        }
        [Authorize(Policy = "readpolicy")]
        public IActionResult Index()
        {
            var roles = roleManager.Roles.ToList();
            return View(roles);
        }
        [Authorize(Policy = "writepolicy")]
        public IActionResult Create()
        {
            return View(new IdentityRole());
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            await roleManager.CreateAsync(role);
            return RedirectToAction("Index");
        }
        [Authorize(Policy = "writepolicy")]
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var role = _db.Roles.Where(d => d.Id == id).FirstOrDefault();
            if ( role != null )
            {
                _db.Roles.Remove(role);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
