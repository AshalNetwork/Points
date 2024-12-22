using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleCrm.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSalesPart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Accepted",
                table: "Sales",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accepted",
                table: "Sales");
        }
    }
}
