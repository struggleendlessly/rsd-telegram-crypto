﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dbMigration;

#nullable disable

namespace dbMigration.Migrations
{
    [DbContext(typeof(solDB))]
    [Migration("20250108165310_In5")]
    partial class In5
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("dbMigration.models.slots", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("numberInt")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("numberInt")
                        .IsUnique();

                    b.ToTable("slotsEntities");
                });

            modelBuilder.Entity("dbMigration.models.swapsTokens", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("addressToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("from")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isBuySol")
                        .HasColumnType("bit");

                    b.Property<bool>("isBuyToken")
                        .HasColumnType("bit");

                    b.Property<double>("priceSol_USD")
                        .HasColumnType("float");

                    b.Property<double>("priceTokenInSol")
                        .HasColumnType("float");

                    b.Property<decimal>("slotNumberEndInt")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("slotNumberStartInt")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("solIn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("solOut")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("to")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("tokenIn")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("tokenOut")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("txsHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("slotNumberEndInt");

                    b.HasIndex("slotNumberStartInt");

                    b.ToTable("swapsTokensEntities");
                });
#pragma warning restore 612, 618
        }
    }
}