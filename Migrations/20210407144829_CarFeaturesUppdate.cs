using Microsoft.EntityFrameworkCore.Migrations;

namespace Auctioneer.Migrations
{
    public partial class CarFeaturesUppdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "CarFeatures",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AuctionCarFeatures",
                columns: table => new
                {
                    AuctionCarFeaturesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuctionID = table.Column<int>(type: "int", nullable: false),
                    CarFeaturesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionCarFeatures", x => x.AuctionCarFeaturesID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionCarFeatures");

            migrationBuilder.DropColumn(
                name: "IsSelected",
                table: "CarFeatures");
        }
    }
}
