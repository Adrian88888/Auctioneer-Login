using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Auctioneer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Auctioneer.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Auction> Auction { get; set; }
        public DbSet<CarBrand> CarBrand { get; set; }
        public DbSet<CarType> CarType { get; set; }
        public DbSet<Gallery> Gallery { get; set; }
        public DbSet<Bids> Bids { get; set; }
    }
}
