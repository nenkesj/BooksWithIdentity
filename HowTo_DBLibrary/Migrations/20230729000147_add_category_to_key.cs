using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HowTo_DBLibrary.Migrations
{
    public partial class add_category_to_key : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Keys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Keys");
        }
    }
}
