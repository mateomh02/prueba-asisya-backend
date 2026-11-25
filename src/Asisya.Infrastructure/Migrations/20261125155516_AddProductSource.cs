using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asisya.Infrastructure.Migrations;

/// <inheritdoc />
public partial class _20261125155516_AddProductSource : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Source",
            table: "Product",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "Manual");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Source",
            table: "Product");
    }
}
