using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HowTo_DBLibrary.Migrations
{
    public partial class add_owner_to_nodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "Nodes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Nodes");
        }
    }
}
