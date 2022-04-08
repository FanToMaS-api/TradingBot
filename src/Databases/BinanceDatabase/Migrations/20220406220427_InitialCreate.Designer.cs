﻿// <auto-generated />
using System;
using BinanceDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinanceDatabase.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220406220427_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.15")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("BinanceDatabase.Entities.HotMiniTickerEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Pair")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("pair");

                    b.Property<double>("Price")
                        .HasColumnType("double precision")
                        .HasColumnName("price");

                    b.Property<DateTime>("ReceivedTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("received_time");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasDatabaseName("IX_hot_mini_tickers_id");

                    b.HasIndex("Pair")
                        .HasDatabaseName("IX_hot_mini_tickers_pair");

                    b.HasIndex("ReceivedTime")
                        .HasDatabaseName("IX_hot_mini_tickers_received_time");

                    b.ToTable("hot_mini_tickers");
                });

            modelBuilder.Entity("BinanceDatabase.Entities.MiniTickerEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("BasePurchaseVolume")
                        .HasColumnType("double precision")
                        .HasColumnName("base_purchase_volume");

                    b.Property<double>("ClosePrice")
                        .HasColumnType("double precision")
                        .HasColumnName("close_price");

                    b.Property<DateTime>("EventTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("event_time");

                    b.Property<int>("IntervalType")
                        .HasColumnType("integer")
                        .HasColumnName("interval");

                    b.Property<double>("MaxPrice")
                        .HasColumnType("double precision")
                        .HasColumnName("max_price");

                    b.Property<double>("MinPrice")
                        .HasColumnType("double precision")
                        .HasColumnName("min_price");

                    b.Property<double>("OpenPrice")
                        .HasColumnType("double precision")
                        .HasColumnName("open_price");

                    b.Property<double>("QuotePurchaseVolume")
                        .HasColumnType("double precision")
                        .HasColumnName("quote_purchase_volume");

                    b.Property<string>("ShortName")
                        .HasColumnType("text")
                        .HasColumnName("short_name");

                    b.HasKey("Id");

                    b.HasIndex("ClosePrice")
                        .HasDatabaseName("IX_mini_tickers_close_price");

                    b.HasIndex("EventTime")
                        .HasDatabaseName("IX_mini_tickers_event_time");

                    b.HasIndex("IntervalType")
                        .HasDatabaseName("IX_mini_tickers_interval");

                    b.HasIndex("MaxPrice")
                        .HasDatabaseName("IX_mini_tickers_max_price");

                    b.HasIndex("MinPrice")
                        .HasDatabaseName("IX_mini_tickers_min_price");

                    b.HasIndex("OpenPrice")
                        .HasDatabaseName("IX_mini_tickers_open_price");

                    b.HasIndex("ShortName")
                        .HasDatabaseName("IX_mini_tickers_short_name");

                    b.ToTable("mini_tickers");
                });
#pragma warning restore 612, 618
        }
    }
}
