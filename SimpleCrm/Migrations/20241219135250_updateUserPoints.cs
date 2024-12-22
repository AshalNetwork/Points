using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleCrm.Migrations
{
    /// <inheritdoc />
    public partial class updateUserPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PointType",
                table: "UserPoints",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PointType",
                table: "UserPoints");
        }
    }
}
