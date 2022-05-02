using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramServiceDatabase.Migrations
{
    public partial class RemoveBalanceFromUserStateEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_state_balance",
                table: "users_state");

            migrationBuilder.DropColumn(
                name: "balance",
                table: "users_state");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "balance",
                table: "users_state",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_users_state_balance",
                table: "users_state",
                column: "balance");
        }
    }
}
