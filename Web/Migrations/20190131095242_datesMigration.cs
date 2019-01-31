using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class datesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SubmissionDate",
                table: "Assignment",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmissionDate",
                table: "Assignment");
        }
    }
}
