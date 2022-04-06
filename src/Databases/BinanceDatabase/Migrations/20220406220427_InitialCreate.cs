using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinanceDatabase.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hot_mini_tickers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pair = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    received_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hot_mini_tickers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mini_tickers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    short_name = table.Column<string>(type: "text", nullable: true),
                    event_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    interval = table.Column<int>(type: "integer", nullable: false),
                    close_price = table.Column<double>(type: "double precision", nullable: false),
                    open_price = table.Column<double>(type: "double precision", nullable: false),
                    min_price = table.Column<double>(type: "double precision", nullable: false),
                    max_price = table.Column<double>(type: "double precision", nullable: false),
                    base_purchase_volume = table.Column<double>(type: "double precision", nullable: false),
                    quote_purchase_volume = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mini_tickers", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hot_mini_tickers_id",
                table: "hot_mini_tickers",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_hot_mini_tickers_pair",
                table: "hot_mini_tickers",
                column: "pair");

            migrationBuilder.CreateIndex(
                name: "IX_hot_mini_tickers_received_time",
                table: "hot_mini_tickers",
                column: "received_time");

            migrationBuilder.CreateIndex(
                name: "IX_mini_tickers_close_price",
                table: "mini_tickers",
                column: "close_price");

            migrationBuilder.CreateIndex(
                name: "IX_mini_tickers_event_time",
                table: "mini_tickers",
                column: "event_time");

            migrationBuilder.CreateIndex(
                name: "IX_mini_tickers_interval",
                table: "mini_tickers",
                column: "interval");

            migrationBuilder.CreateIndex(
                name: "IX_mini_tickers_max_price",
                table: "mini_tickers",
                column: "max_price");

            migrationBuilder.CreateIndex(
                name: "IX_mini_tickers_min_price",
                table: "mini_tickers",
                column: "min_price");

            migrationBuilder.CreateIndex(
                name: "IX_mini_tickers_open_price",
                table: "mini_tickers",
                column: "open_price");

            migrationBuilder.CreateIndex(
                name: "IX_mini_tickers_short_name",
                table: "mini_tickers",
                column: "short_name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hot_mini_tickers");

            migrationBuilder.DropTable(
                name: "mini_tickers");
        }
    }
}
