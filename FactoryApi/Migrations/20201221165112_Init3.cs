using Microsoft.EntityFrameworkCore.Migrations;

namespace FactoryApi.Migrations
{
    public partial class Init3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrinterName",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WriterName",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrinterName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WriterName",
                table: "Orders");
        }
    }
}
