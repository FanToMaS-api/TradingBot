using Microsoft.EntityFrameworkCore.Migrations;

namespace BinanceDatabase.Migrations
{
    public partial class RenameAggregateDataIntervalColumnInMiniTickerEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "interval",
                table: "mini_tickers",
                newName: "aggregate_data_interval");

            migrationBuilder.RenameIndex(
                name: "IX_mini_tickers_interval",
                table: "mini_tickers",
                newName: "IX_mini_tickers_aggregate_data_interval");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "aggregate_data_interval",
                table: "mini_tickers",
                newName: "interval");

            migrationBuilder.RenameIndex(
                name: "IX_mini_tickers_aggregate_data_interval",
                table: "mini_tickers",
                newName: "IX_mini_tickers_interval");
        }
    }
}
