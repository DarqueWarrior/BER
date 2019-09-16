using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BerService.Controllers.DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    ApplicationName = table.Column<string>(nullable: false),
                    Version = table.Column<string>(nullable: false),
                    DataType = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => new { x.ApplicationName, x.DataType, x.Version });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Records");
        }
    }
}
