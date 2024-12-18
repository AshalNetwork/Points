using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleCrm.Migrations
{
    /// <inheritdoc />
    public partial class AddCommingReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CommingReasonId",
                table: "Clients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomReason",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CommingReasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommingReasons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CommingReasonId",
                table: "Clients",
                column: "CommingReasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_CommingReasons_CommingReasonId",
                table: "Clients",
                column: "CommingReasonId",
                principalTable: "CommingReasons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_CommingReasons_CommingReasonId",
                table: "Clients");

            migrationBuilder.DropTable(
                name: "CommingReasons");

            migrationBuilder.DropIndex(
                name: "IX_Clients_CommingReasonId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "CommingReasonId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "CustomReason",
                table: "Clients");
        }
    }
}
