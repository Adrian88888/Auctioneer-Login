using Microsoft.EntityFrameworkCore.Migrations;

namespace Auctioneer.Migrations
{
    public partial class AddedCarFeatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarFeatures",
                columns: table => new
                {
                    CarFeatureID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarFeature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuctionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarFeatures", x => x.CarFeatureID);
                    table.ForeignKey(
                        name: "FK_CarFeatures_Auction_AuctionID",
                        column: x => x.AuctionID,
                        principalTable: "Auction",
                        principalColumn: "AuctionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarFeatures_AuctionID",
                table: "CarFeatures",
                column: "AuctionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarFeatures");
        }
    }
}
