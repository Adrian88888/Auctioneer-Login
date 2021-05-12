using Database.Data;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auctioneer.Services.BidsServices
{
    public class BidService
    {
        public int GetUserLastBid(ApplicationDbContext _db, string userID, int auctionID)
        {
            List<Bids> bids = _db.Bids.ToList();
            foreach (var bid in bids)
            {
                if (bid.UserID == userID && bid.AuctionID == auctionID)
                {
                    return bid.Amount;
                }
            }
            return 0;
        }
        public List<Bids> GetAllUserBids(ApplicationDbContext _db, string userID)
        {
            List<Bids> allUserBids = new();
            List<Bids> bids = _db.Bids.ToList();
            foreach (var bid in bids)
            {
                if (bid.UserID == userID)
                {
                    allUserBids.Add(bid);
                }
            }
            return allUserBids;
        }
    }
}
