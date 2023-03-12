using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EthioTelQuizBotBusinessLogic.Migrations
{
    public partial class modelModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Choices",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "CorrectAnswer",
                table: "Answers",
                newName: "Choice");

            migrationBuilder.AddColumn<string>(
                name: "CorrectAnswer",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectAnswer",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "Choice",
                table: "Answers",
                newName: "CorrectAnswer");

            migrationBuilder.AddColumn<string[]>(
                name: "Choices",
                table: "Answers",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }
    }
}
