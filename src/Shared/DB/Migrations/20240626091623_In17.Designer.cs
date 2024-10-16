﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.DB;

#nullable disable

namespace Shared.DB.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20240626091623_In17")]
    partial class In17
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Shared.DB.TokenInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AddressOwnersWallet")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AddressToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BlockNumber")
                        .HasColumnType("int");

                    b.Property<string>("ErrorType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HashContractTransaction")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsProcessed1")
                        .HasColumnType("bit");

                    b.Property<bool>("IsProcessed2")
                        .HasColumnType("bit");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<string>("NameToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TellMessageIdBotVerified")
                        .HasColumnType("int");

                    b.Property<int>("TellMessageIdIsValid")
                        .HasColumnType("int");

                    b.Property<int>("TellMessageIdNotVerified")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("TimeUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("UrlChart")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrlOwnersWallet")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrlToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("bitcointalk")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("blog")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("blueCheckmark")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("discord")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("divisor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("facebook")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("github")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("linkedin")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("reddit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("slack")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("symbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("telegram")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("tokenPriceUSD")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("tokenType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("totalSupply")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("twitter")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("website")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("wechat")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("whitepaper")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TokenInfos");
                });
#pragma warning restore 612, 618
        }
    }
}
