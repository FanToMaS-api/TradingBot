using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TelegramServiceDatabase.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    telegram_id = table.Column<long>(type: "bigint", nullable: false),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    nickname = table.Column<string>(type: "text", nullable: true),
                    lastname = table.Column<string>(type: "text", nullable: true),
                    firstname = table.Column<string>(type: "text", nullable: true),
                    last_action = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users_state",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    balance = table.Column<double>(type: "double precision", nullable: false),
                    state_type = table.Column<string>(type: "text", nullable: false),
                    ban_reason = table.Column<string>(type: "text", nullable: false),
                    warning_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_state", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_state_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_chat_id",
                table: "users",
                column: "chat_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_firstname",
                table: "users",
                column: "firstname");

            migrationBuilder.CreateIndex(
                name: "IX_users_id",
                table: "users",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_last_action",
                table: "users",
                column: "last_action");

            migrationBuilder.CreateIndex(
                name: "IX_users_lastname",
                table: "users",
                column: "lastname");

            migrationBuilder.CreateIndex(
                name: "IX_users_nickname",
                table: "users",
                column: "nickname");

            migrationBuilder.CreateIndex(
                name: "IX_users_telegram_id",
                table: "users",
                column: "telegram_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_state_balance",
                table: "users_state",
                column: "balance");

            migrationBuilder.CreateIndex(
                name: "IX_users_state_ban_reason",
                table: "users_state",
                column: "ban_reason");

            migrationBuilder.CreateIndex(
                name: "IX_users_state_id",
                table: "users_state",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_state_state_type",
                table: "users_state",
                column: "state_type");

            migrationBuilder.CreateIndex(
                name: "IX_users_state_user_id",
                table: "users_state",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_state_warning_number",
                table: "users_state",
                column: "warning_number");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users_state");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
