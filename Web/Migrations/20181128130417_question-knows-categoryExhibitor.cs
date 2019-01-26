using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class questionknowscategoryExhibitor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExhibitorNumber",
                table: "Exhibitor",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExhibitorNumber",
                table: "Exhibitor");
        }
    }
}
