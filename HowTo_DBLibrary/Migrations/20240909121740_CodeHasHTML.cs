using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HowTo_DBLibrary.Migrations
{
    public partial class CodeHasHTML : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CodeHasHTML",
                table: "Nodes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeHasHTML",
                table: "Nodes");
        }
    }
}
