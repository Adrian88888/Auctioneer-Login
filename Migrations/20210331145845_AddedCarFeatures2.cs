using Microsoft.EntityFrameworkCore.Migrations;

namespace Auctioneer.Migrations
{
    public partial class AddedCarFeatures2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarFeatures_Auction_AuctionID",
                table: "CarFeatures");

            migrationBuilder.DropIndex(
                name: "IX_CarFeatures_AuctionID",
                table: "CarFeatures");

            migrationBuilder.DropColumn(
                name: "AuctionID",
                table: "CarFeatures");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuctionID",
                table: "CarFeatures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CarFeatures_AuctionID",
                table: "CarFeatures",
                column: "AuctionID");

            migrationBuilder.AddForeignKey(
                name: "FK_CarFeatures_Auction_AuctionID",
                table: "CarFeatures",
                column: "AuctionID",
                principalTable: "Auction",
                principalColumn: "AuctionID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
