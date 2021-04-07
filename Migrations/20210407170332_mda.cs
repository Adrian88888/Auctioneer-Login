using Microsoft.EntityFrameworkCore.Migrations;

namespace Auctioneer.Migrations
{
    public partial class mda : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AuctionID",
                table: "AuctionCarFeatures",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionCarFeatures_AuctionID",
                table: "AuctionCarFeatures",
                column: "AuctionID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionCarFeatures_Auction_AuctionID",
                table: "AuctionCarFeatures",
                column: "AuctionID",
                principalTable: "Auction",
                principalColumn: "AuctionID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionCarFeatures_Auction_AuctionID",
                table: "AuctionCarFeatures");

            migrationBuilder.DropIndex(
                name: "IX_AuctionCarFeatures_AuctionID",
                table: "AuctionCarFeatures");

            migrationBuilder.AlterColumn<int>(
                name: "AuctionID",
                table: "AuctionCarFeatures",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
