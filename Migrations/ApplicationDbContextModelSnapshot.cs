﻿// <auto-generated />
using System;
using Auctioneer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Auctioneer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Auctioneer.Models.Auction", b =>
                {
                    b.Property<int>("AuctionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CarBrandID")
                        .HasColumnType("int");

                    b.Property<int>("CarTypeID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<int>("GalleryID")
                        .HasColumnType("int");

                    b.Property<int>("Max_bid")
                        .HasColumnType("int");

                    b.Property<int>("Min_bid")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AuctionID");

                    b.HasIndex("CarBrandID");

                    b.HasIndex("CarTypeID");

                    b.ToTable("Auction");
                });

            modelBuilder.Entity("Auctioneer.Models.CarBrand", b =>
                {
                    b.Property<int>("CarBrandID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Brand")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CarBrandID");

                    b.ToTable("CarBrand");
                });

            modelBuilder.Entity("Auctioneer.Models.CarType", b =>
                {
                    b.Property<int>("CarTypeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CarBrandID")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CarTypeID");

                    b.ToTable("CarType");
                });

            modelBuilder.Entity("Auctioneer.Models.Gallery", b =>
                {
                    b.Property<int>("GalleryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AuctionID")
                        .HasColumnType("int");

                    b.Property<string>("ImageName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GalleryID");

                    b.HasIndex("AuctionID");

                    b.ToTable("Gallery");
                });

            modelBuilder.Entity("Auctioneer.Models.Auction", b =>
                {
                    b.HasOne("Auctioneer.Models.CarBrand", "CarBrand")
                        .WithMany()
                        .HasForeignKey("CarBrandID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Auctioneer.Models.CarType", "CarType")
                        .WithMany()
                        .HasForeignKey("CarTypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CarBrand");

                    b.Navigation("CarType");
                });

            modelBuilder.Entity("Auctioneer.Models.Gallery", b =>
                {
                    b.HasOne("Auctioneer.Models.Auction", null)
                        .WithMany("Gallery")
                        .HasForeignKey("AuctionID");
                });

            modelBuilder.Entity("Auctioneer.Models.Auction", b =>
                {
                    b.Navigation("Gallery");
                });
#pragma warning restore 612, 618
        }
    }
}
