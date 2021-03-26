using Microsoft.EntityFrameworkCore.Migrations;

namespace Auctioneer.Migrations
{
    public partial class CloseToFinalUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Min_bid",
                table: "Auction",
                newName: "MinBid");

            migrationBuilder.RenameColumn(
                name: "Max_bid",
                table: "Auction",
                newName: "MaxBid");

            migrationBuilder.RenameColumn(
                name: "Current_bid",
                table: "Auction",
                newName: "CurrentBid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinBid",
                table: "Auction",
                newName: "Min_bid");

            migrationBuilder.RenameColumn(
                name: "MaxBid",
                table: "Auction",
                newName: "Max_bid");

            migrationBuilder.RenameColumn(
                name: "CurrentBid",
                table: "Auction",
                newName: "Current_bid");
        }
    }
}
