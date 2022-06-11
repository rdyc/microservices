using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Product.Domain.Migrations.Postgre.Product
{
    public partial class InitProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "item");

            migrationBuilder.CreateTable(
                name: "main",
                schema: "item",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_main", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "main",
                schema: "item");
        }
    }
}
