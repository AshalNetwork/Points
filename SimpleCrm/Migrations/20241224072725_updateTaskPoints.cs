using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleCrm.Migrations
{
    /// <inheritdoc />
    public partial class updateTaskPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPoints_Tasks_TaskId",
                table: "UserPoints");

            migrationBuilder.DropIndex(
                name: "IX_UserPoints_TaskId",
                table: "UserPoints");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "UserPoints");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TaskId",
                table: "UserPoints",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPoints_TaskId",
                table: "UserPoints",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPoints_Tasks_TaskId",
                table: "UserPoints",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
