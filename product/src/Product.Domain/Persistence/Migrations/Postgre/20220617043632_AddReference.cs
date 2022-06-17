using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Product.Domain.Persistence.Migrations.Postgre
{
    public partial class AddReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_currency_main_ProductId",
                schema: "product",
                table: "currency");

            migrationBuilder.DropPrimaryKey(
                name: "PK_currency",
                schema: "product",
                table: "currency");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "product",
                table: "main");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "product",
                table: "attribute");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "product",
                table: "attribute");

            migrationBuilder.DropColumn(
                name: "Unit",
                schema: "product",
                table: "attribute");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "config",
                table: "currency");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "config",
                table: "attribute");

            migrationBuilder.DropColumn(
                name: "Value",
                schema: "config",
                table: "attribute");

            migrationBuilder.DropColumn(
                name: "ProductId",
                schema: "product",
                table: "currency");

            migrationBuilder.EnsureSchema(
                name: "reference");

            migrationBuilder.RenameTable(
                name: "currency",
                schema: "product",
                newName: "currency",
                newSchema: "reference");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyRefId",
                schema: "product",
                table: "main",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "product",
                table: "main",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "AttributeRefId",
                schema: "product",
                table: "attribute",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "config",
                table: "currency",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "config",
                table: "attribute",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RefId",
                schema: "reference",
                table: "currency",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "reference",
                table: "currency",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_currency",
                schema: "reference",
                table: "currency",
                column: "RefId");

            migrationBuilder.CreateTable(
                name: "attribute",
                schema: "reference",
                columns: table => new
                {
                    RefId = table.Column<Guid>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Unit = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attribute", x => x.RefId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_main_CurrencyRefId",
                schema: "product",
                table: "main",
                column: "CurrencyRefId");

            migrationBuilder.CreateIndex(
                name: "IX_attribute_AttributeRefId",
                schema: "product",
                table: "attribute",
                column: "AttributeRefId");

            migrationBuilder.CreateIndex(
                name: "IX_currency_Id_Code_Name_Symbol",
                schema: "reference",
                table: "currency",
                columns: new[] { "Id", "Code", "Name", "Symbol" });

            migrationBuilder.CreateIndex(
                name: "IX_attribute_Id_Name_Type_Unit",
                schema: "reference",
                table: "attribute",
                columns: new[] { "Id", "Name", "Type", "Unit" });

            migrationBuilder.AddForeignKey(
                name: "FK_attribute_attribute_AttributeRefId",
                schema: "product",
                table: "attribute",
                column: "AttributeRefId",
                principalSchema: "reference",
                principalTable: "attribute",
                principalColumn: "RefId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_main_currency_CurrencyRefId",
                schema: "product",
                table: "main",
                column: "CurrencyRefId",
                principalSchema: "reference",
                principalTable: "currency",
                principalColumn: "RefId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attribute_attribute_AttributeRefId",
                schema: "product",
                table: "attribute");

            migrationBuilder.DropForeignKey(
                name: "FK_main_currency_CurrencyRefId",
                schema: "product",
                table: "main");

            migrationBuilder.DropTable(
                name: "attribute",
                schema: "reference");

            migrationBuilder.DropIndex(
                name: "IX_main_CurrencyRefId",
                schema: "product",
                table: "main");

            migrationBuilder.DropIndex(
                name: "IX_attribute_AttributeRefId",
                schema: "product",
                table: "attribute");

            migrationBuilder.DropPrimaryKey(
                name: "PK_currency",
                schema: "reference",
                table: "currency");

            migrationBuilder.DropIndex(
                name: "IX_currency_Id_Code_Name_Symbol",
                schema: "reference",
                table: "currency");

            migrationBuilder.DropColumn(
                name: "CurrencyRefId",
                schema: "product",
                table: "main");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "product",
                table: "main");

            migrationBuilder.DropColumn(
                name: "AttributeRefId",
                schema: "product",
                table: "attribute");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "config",
                table: "currency");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "config",
                table: "attribute");

            migrationBuilder.DropColumn(
                name: "RefId",
                schema: "reference",
                table: "currency");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "reference",
                table: "currency");

            migrationBuilder.RenameTable(
                name: "currency",
                schema: "reference",
                newName: "currency",
                newSchema: "product");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "product",
                table: "main",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "product",
                table: "attribute",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "product",
                table: "attribute",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                schema: "product",
                table: "attribute",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "config",
                table: "currency",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "config",
                table: "attribute",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                schema: "config",
                table: "attribute",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                schema: "product",
                table: "currency",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_currency",
                schema: "product",
                table: "currency",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_currency_main_ProductId",
                schema: "product",
                table: "currency",
                column: "ProductId",
                principalSchema: "product",
                principalTable: "main",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
