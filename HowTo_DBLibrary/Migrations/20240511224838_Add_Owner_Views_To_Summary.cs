using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HowTo_DBLibrary.Migrations
{
    public partial class Add_Owner_Views_To_Summary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "Summaries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Views",
                table: "Summaries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "Nodes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<long>(
                name: "Views",
                table: "Nodes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "Views",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "Views",
                table: "Nodes");

            migrationBuilder.AlterColumn<string>(
                name: "Owner",
                table: "Nodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
