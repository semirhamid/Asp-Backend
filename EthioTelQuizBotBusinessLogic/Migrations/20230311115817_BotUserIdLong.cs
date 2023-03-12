using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EthioTelQuizBotBusinessLogic.Migrations
{
    public partial class BotUserIdLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "BotUserId",
                table: "Subscribers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BotUserId",
                table: "Subscribers",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
