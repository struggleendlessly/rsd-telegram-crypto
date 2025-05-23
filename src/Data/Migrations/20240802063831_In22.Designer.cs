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
    [Migration("20240802063831_In22")]
    partial class In22
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

                    b.Property<bool>("exploitsTS")
                        .HasColumnType("bit");

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

                    b.Property<string>("r")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("s")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("symbol")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("tlgrmNewTokens")
                        .HasColumnType("int");

                    b.Property<string>("to")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("totalSupply")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("transactionIndex")
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
#pragma warning restore 612, 618
        }
    }
}
