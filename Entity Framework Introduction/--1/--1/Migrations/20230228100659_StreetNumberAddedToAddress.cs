using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace __1.Migrations
{
    public partial class StreetNumberAddedToAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StreetNumber",
                table: "Addresses",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreetNumber",
                table: "Addresses");
        }
    }
}
