﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(dbContext))]
    [Migration("20240919171911_In45")]
    partial class In45
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Data.Models.EthBlocks", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("baseFeePerGas")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("gasLimit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("gasUsed")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("number")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("numberInt")
                        .HasColumnType("int");

                    b.Property<string>("timestamp")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("numberInt")
                        .IsUnique();

                    b.ToTable("EthBlock");
                });

            modelBuilder.Entity("Data.Models.EthSwapEvents", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("EthIn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EthOut")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("EthTrainDataId")
                        .HasColumnType("int");

                    b.Property<string>("TokenIn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TokenOut")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("blockNumberInt")
                        .HasColumnType("int");

                    b.Property<string>("from")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isBuy")
                        .HasColumnType("bit");

                    b.Property<string>("pairAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("priceEth")
                        .HasColumnType("float");

                    b.Property<string>("to")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("tokenNotEth")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("txsHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EthTrainDataId");

                    b.HasIndex("blockNumberInt");

                    b.ToTable("EthSwapEvents");
                });

            modelBuilder.Entity("Data.Models.EthSwapEventsETHUSD", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("EthIn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EthOut")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TokenIn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TokenOut")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("blockNumberInt")
                        .HasColumnType("int");

                    b.Property<string>("from")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isBuyDai")
                        .HasColumnType("bit");

                    b.Property<bool>("isBuyEth")
                        .HasColumnType("bit");

                    b.Property<string>("pairAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("priceEthInUsd")
                        .HasColumnType("float");

                    b.Property<string>("to")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("txsHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("blockNumberInt");

                    b.ToTable("EthSwapEventsETHUSD");
                });

            modelBuilder.Entity("Data.Models.EthTokensVolume", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("EthTrainDataId")
                        .HasColumnType("int");

                    b.Property<int>("blockIntEnd")
                        .HasColumnType("int");

                    b.Property<int>("blockIntStart")
                        .HasColumnType("int");

                    b.Property<bool>("isTlgrmMessageSent")
                        .HasColumnType("bit");

                    b.Property<int>("periodInMins")
                        .HasColumnType("int");

                    b.Property<string>("volumeNegativeEth")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("volumePositiveEth")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("volumeTotalEth")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EthTrainDataId");

                    b.HasIndex("blockIntEnd");

                    b.ToTable("EthTokensVolumes");
                });

            modelBuilder.Entity("Data.Models.EthTrainData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ABI")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("BalanceOnCreating")
                        .HasColumnType("float");

                    b.Property<string>("CompilerVersion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConstructorArguments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContractName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DeadBlockNumber")
                        .HasColumnType("int");

                    b.Property<string>("EVMVersion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Implementation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Library")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LicenseType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OptimizationUsed")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Proxy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Runs")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SourceCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SwarmSource")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WalletSource1in")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WalletSource1inCountRemLiq")
                        .HasColumnType("int");

                    b.Property<string>("blockHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("blockNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("blockNumberInt")
                        .HasColumnType("int");

                    b.Property<string>("chainId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("contractAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("decimals")
                        .HasColumnType("int");

                    b.Property<string>("from")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("gas")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("gasPrice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("hash")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("input")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isCustomInputStart")
                        .HasColumnType("bit");

                    b.Property<bool>("isDead")
                        .HasColumnType("bit");

                    b.Property<int>("isDeadInt")
                        .HasColumnType("int");

                    b.Property<string>("logo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("logs")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("maxFeePerBlobGas")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("maxFeePerGas")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("maxPriorityFeePerGas")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("nonce")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pairAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pairAddressFunctionName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("pairBlockNumberInt")
                        .HasColumnType("int");

                    b.Property<string>("r")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("s")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("symbol")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("tlgrmLivePairs")
                        .HasColumnType("bigint");

                    b.Property<long>("tlgrmNewTokens")
                        .HasColumnType("bigint");

                    b.Property<long>("tlgrmVolume")
                        .HasColumnType("bigint");

                    b.Property<string>("to")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("totalSupply")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("transactionIndex")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("tsExploits")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("tsFullResponse")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("v")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("value")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("walletCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("yParity")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("blockNumberInt");

                    b.HasIndex("contractAddress")
                        .IsUnique();

                    b.HasIndex("hash")
                        .IsUnique();

                    b.ToTable("EthTrainData");
                });

            modelBuilder.Entity("Data.Models.EthTrxOthers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("blockHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("blockNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("blockNumberInt")
                        .HasColumnType("int");

                    b.Property<string>("chainId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("from")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("gas")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("gasPrice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("hash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("input")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("maxFeePerBlobGas")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("maxFeePerGas")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("maxPriorityFeePerGas")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("nonce")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("r")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("s")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("to")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("transactionIndex")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("v")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("value")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("yParity")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("EthTrxOther");
                });

            modelBuilder.Entity("Data.Models.TradeCompanies", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TradeCompanies");
                });

            modelBuilder.Entity("Data.Models.WalletNames", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TradeCompaniesId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TradeCompaniesId");

                    b.ToTable("WalletNames");
                });

            modelBuilder.Entity("Data.Models.EthSwapEvents", b =>
                {
                    b.HasOne("Data.Models.EthTrainData", "EthTrainData")
                        .WithMany("EthSwapEvents")
                        .HasForeignKey("EthTrainDataId");

                    b.Navigation("EthTrainData");
                });

            modelBuilder.Entity("Data.Models.EthTokensVolume", b =>
                {
                    b.HasOne("Data.Models.EthTrainData", "EthTrainData")
                        .WithMany("EthTokensVolumes")
                        .HasForeignKey("EthTrainDataId");

                    b.Navigation("EthTrainData");
                });

            modelBuilder.Entity("Data.Models.WalletNames", b =>
                {
                    b.HasOne("Data.Models.TradeCompanies", "TradeCompanies")
                        .WithMany("WalletNames")
                        .HasForeignKey("TradeCompaniesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TradeCompanies");
                });

            modelBuilder.Entity("Data.Models.EthTrainData", b =>
                {
                    b.Navigation("EthSwapEvents");

                    b.Navigation("EthTokensVolumes");
                });

            modelBuilder.Entity("Data.Models.TradeCompanies", b =>
                {
                    b.Navigation("WalletNames");
                });
#pragma warning restore 612, 618
        }
    }
}
