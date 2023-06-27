﻿// <auto-generated />
using System;
using Artillery.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Artillery.Migrations
{
    [DbContext(typeof(ArtilleryContext))]
    [Migration("20230328100724_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Artillery.Data.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ArmySize")
                        .HasColumnType("int");

                    b.Property<string>("CountryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("Artillery.Data.Models.CountryGun", b =>
                {
                    b.Property<int>("CountryId")
                        .HasColumnType("int");

                    b.Property<int>("GunId")
                        .HasColumnType("int");

                    b.HasKey("CountryId", "GunId");

                    b.HasIndex("GunId");

                    b.ToTable("CountriesGuns");
                });

            modelBuilder.Entity("Artillery.Data.Models.Gun", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<double>("BarrelLength")
                        .HasColumnType("float");

                    b.Property<int>("GunType")
                        .HasColumnType("int");

                    b.Property<int>("GunWeight")
                        .HasColumnType("int");

                    b.Property<int>("ManufacturerId")
                        .HasColumnType("int");

                    b.Property<int?>("NumberBuild")
                        .HasColumnType("int");

                    b.Property<int>("Range")
                        .HasColumnType("int");

                    b.Property<int>("ShellId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ManufacturerId");

                    b.HasIndex("ShellId");

                    b.ToTable("Guns");
                });

            modelBuilder.Entity("Artillery.Data.Models.Manufacturer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Founded")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ManufacturerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Manufacturers");
                });

            modelBuilder.Entity("Artillery.Data.Models.Shell", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Caliber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ShellWeight")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Shells");
                });

            modelBuilder.Entity("Artillery.Data.Models.CountryGun", b =>
                {
                    b.HasOne("Artillery.Data.Models.Country", "Country")
                        .WithMany("CountriesGuns")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Artillery.Data.Models.Gun", "Gun")
                        .WithMany("CountriesGuns")
                        .HasForeignKey("GunId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");

                    b.Navigation("Gun");
                });

            modelBuilder.Entity("Artillery.Data.Models.Gun", b =>
                {
                    b.HasOne("Artillery.Data.Models.Manufacturer", "Manufacturer")
                        .WithMany("Guns")
                        .HasForeignKey("ManufacturerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Artillery.Data.Models.Shell", "Shell")
                        .WithMany("Guns")
                        .HasForeignKey("ShellId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Manufacturer");

                    b.Navigation("Shell");
                });

            modelBuilder.Entity("Artillery.Data.Models.Country", b =>
                {
                    b.Navigation("CountriesGuns");
                });

            modelBuilder.Entity("Artillery.Data.Models.Gun", b =>
                {
                    b.Navigation("CountriesGuns");
                });

            modelBuilder.Entity("Artillery.Data.Models.Manufacturer", b =>
                {
                    b.Navigation("Guns");
                });

            modelBuilder.Entity("Artillery.Data.Models.Shell", b =>
                {
                    b.Navigation("Guns");
                });
#pragma warning restore 612, 618
        }
    }
}
