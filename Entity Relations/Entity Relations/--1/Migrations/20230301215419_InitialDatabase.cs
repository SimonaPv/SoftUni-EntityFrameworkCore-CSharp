using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace __1.Migrations
{
    public partial class InitialDatabase : Migration
    {
        public DbSet<Car> Cars { get; set; } 

        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
