using Microsoft.EntityFrameworkCore.Migrations;

namespace BinanceDatabase.Migrations
{
    public partial class AddPriceDeviationToMinTickerEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "price_deviation",
                table: "mini_tickers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_mini_tickers_price_deviation",
                table: "mini_tickers",
                column: "price_deviation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_mini_tickers_price_deviation",
                table: "mini_tickers");

            migrationBuilder.DropColumn(
                name: "price_deviation",
                table: "mini_tickers");
        }
    }
}
