﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Auctioneer.Migrations
{
    public partial class REVERTED : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auction_CarBrand_CarBrandID",
                table: "Auction");

            migrationBuilder.DropForeignKey(
                name: "FK_Auction_CarType_CarTypeID",
                table: "Auction");

            migrationBuilder.AlterColumn<int>(
                name: "CarTypeID",
                table: "Auction",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CarBrandID",
                table: "Auction",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_CarBrand_CarBrandID",
                table: "Auction",
                column: "CarBrandID",
                principalTable: "CarBrand",
                principalColumn: "CarBrandID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_CarType_CarTypeID",
                table: "Auction",
                column: "CarTypeID",
                principalTable: "CarType",
                principalColumn: "CarTypeID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auction_CarBrand_CarBrandID",
                table: "Auction");

            migrationBuilder.DropForeignKey(
                name: "FK_Auction_CarType_CarTypeID",
                table: "Auction");

            migrationBuilder.AlterColumn<int>(
                name: "CarTypeID",
                table: "Auction",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CarBrandID",
                table: "Auction",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_CarBrand_CarBrandID",
                table: "Auction",
                column: "CarBrandID",
                principalTable: "CarBrand",
                principalColumn: "CarBrandID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_CarType_CarTypeID",
                table: "Auction",
                column: "CarTypeID",
                principalTable: "CarType",
                principalColumn: "CarTypeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}