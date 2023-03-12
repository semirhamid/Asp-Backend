using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EthioTelQuizBotBusinessLogic.Migrations
{
    public partial class AddedScoreToProblems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Point",
                table: "Problems",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "Problems",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Problems");

            migrationBuilder.AlterColumn<double>(
                name: "Point",
                table: "Problems",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
