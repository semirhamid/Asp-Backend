using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EthioTelQuizBotBusinessLogic.Migrations
{
    public partial class UpdatedTheSubscriberDataTypeToLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SubscriberId",
                table: "Problems",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SubscriberId",
                table: "Problems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
