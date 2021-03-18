using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Auctioneer.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarBrand",
                columns: table => new
                {
                    CarBrandID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarBrand", x => x.CarBrandID);
                });

            migrationBuilder.CreateTable(
                name: "CarType",
                columns: table => new
                {
                    CarTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarBrandID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarType", x => x.CarTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Auction",
                columns: table => new
                {
                    AuctionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Min_bid = table.Column<int>(type: "int", nullable: false),
                    Max_bid = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CarBrandID = table.Column<int>(type: "int", nullable: false),
                    CarTypeID = table.Column<int>(type: "int", nullable: false),
                    GalleryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auction", x => x.AuctionID);
                    table.ForeignKey(
                        name: "FK_Auction_CarBrand_CarBrandID",
                        column: x => x.CarBrandID,
                        principalTable: "CarBrand",
                        principalColumn: "CarBrandID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Auction_CarType_CarTypeID",
                        column: x => x.CarTypeID,
                        principalTable: "CarType",
                        principalColumn: "CarTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gallery",
                columns: table => new
                {
                    GalleryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuctionID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gallery", x => x.GalleryID);
                    table.ForeignKey(
                        name: "FK_Gallery_Auction_AuctionID",
                        column: x => x.AuctionID,
                        principalTable: "Auction",
                        principalColumn: "AuctionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auction_CarBrandID",
                table: "Auction",
                column: "CarBrandID");

            migrationBuilder.CreateIndex(
                name: "IX_Auction_CarTypeID",
                table: "Auction",
                column: "CarTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Gallery_AuctionID",
                table: "Gallery",
                column: "AuctionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gallery");

            migrationBuilder.DropTable(
                name: "Auction");

            migrationBuilder.DropTable(
                name: "CarBrand");

            migrationBuilder.DropTable(
                name: "CarType");
        }
    }
}
