using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramServiceDatabase.Migrations
{
    public partial class RenameUserStateEntityField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "state_type",
                table: "users_state",
                newName: "status");

            migrationBuilder.RenameIndex(
                name: "IX_users_state_state_type",
                table: "users_state",
                newName: "IX_users_state_status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "users_state",
                newName: "state_type");

            migrationBuilder.RenameIndex(
                name: "IX_users_state_status",
                table: "users_state",
                newName: "IX_users_state_state_type");
        }
    }
}
