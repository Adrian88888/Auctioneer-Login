"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/realtimeDataHub").build();

connection.start();
connection.on("ReceiveHighestBid", function (highestBid, auctionID) {
    document.getElementById(auctionID).innerHTML = 'Highest Bid: $' + highestBid;
});