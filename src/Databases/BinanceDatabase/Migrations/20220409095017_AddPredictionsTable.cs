using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinanceDatabase.Migrations
{
    public partial class AddPredictionsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "predictions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    short_name = table.Column<string>(type: "text", nullable: true),
                    prediction_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    price_values = table.Column<double[]>(type: "double precision[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_predictions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_predictions_prediction_time",
                table: "predictions",
                column: "prediction_time");

            migrationBuilder.CreateIndex(
                name: "IX_predictions_short_name",
                table: "predictions",
                column: "short_name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "predictions");
        }
    }
}
