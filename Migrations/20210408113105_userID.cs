using Microsoft.EntityFrameworkCore.Migrations;

namespace Auctioneer.Migrations
{
    public partial class userID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuctionWinner",
                table: "Auction",
                newName: "AuctionWinnerID");

            migrationBuilder.RenameColumn(
                name: "AuctionOwner",
                table: "Auction",
                newName: "AuctionOwnerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuctionWinnerID",
                table: "Auction",
                newName: "AuctionWinner");

            migrationBuilder.RenameColumn(
                name: "AuctionOwnerID",
                table: "Auction",
                newName: "AuctionOwner");
        }
    }
}
