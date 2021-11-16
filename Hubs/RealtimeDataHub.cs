using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace Auctioneer.Hubs
{
    public class RealtimeDataHub : Hub
    {
        public async Task UpdateHighestBidAsync(int highestBid, int auctionID)
        {
            await Clients.All.SendAsync("ReceiveHighestBid", highestBid, auctionID);
        }
    }
}