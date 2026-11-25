using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Asisya.Infrastructure.Migrations;

/// <inheritdoc />
public partial class _20261124034117_AddProductsAndSuppliers : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_Categories",
            table: "Categories");

        migrationBuilder.RenameTable(
            name: "Categories",
            newName: "Category");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Category",
            table: "Category",
            column: "Id");

        migrationBuilder.CreateTable(
            name: "Supplier",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                CompanyName = table.Column<string>(type: "text", nullable: false),
                ContactName = table.Column<string>(type: "text", nullable: true),
                ContactTitle = table.Column<string>(type: "text", nullable: true),
                Address = table.Column<string>(type: "text", nullable: true),
                City = table.Column<string>(type: "text", nullable: true),
                Region = table.Column<string>(type: "text", nullable: true),
                PostalCode = table.Column<string>(type: "text", nullable: true),
                Country = table.Column<string>(type: "text", nullable: true),
                Phone = table.Column<string>(type: "text", nullable: true),
                Fax = table.Column<string>(type: "text", nullable: true),
                HomePage = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Supplier", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Product",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ProductName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                SupplierId = table.Column<int>(type: "integer", nullable: false),
                CategoryId = table.Column<int>(type: "integer", nullable: false),
                QuantityPerUnit = table.Column<string>(type: "text", nullable: true),
                UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                UnitsInStock = table.Column<short>(type: "smallint", nullable: false),
                UnitsOnOrder = table.Column<short>(type: "smallint", nullable: false),
                ReorderLevel = table.Column<short>(type: "smallint", nullable: false),
                Discontinued = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Product", x => x.Id);
                table.ForeignKey(
                    name: "FK_Product_Category_CategoryId",
                    column: x => x.CategoryId,
                    principalTable: "Category",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Product_Supplier_SupplierId",
                    column: x => x.SupplierId,
                    principalTable: "Supplier",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Product_CategoryId",
            table: "Product",
            column: "CategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Product_SupplierId",
            table: "Product",
            column: "SupplierId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Product");

        migrationBuilder.DropTable(
            name: "Supplier");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Category",
            table: "Category");

        migrationBuilder.RenameTable(
            name: "Category",
            newName: "Categories");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Categories",
            table: "Categories",
            column: "Id");
    }
}
