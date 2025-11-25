using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamoDispositivos.Migrations
{
    /// <inheritdoc />
    public partial class AsignacionUserAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "AdminDisp",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminDisp_ApplicationUserId",
                table: "AdminDisp",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminDisp_Users_ApplicationUserId",
                table: "AdminDisp",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminDisp_Users_ApplicationUserId",
                table: "AdminDisp");

            migrationBuilder.DropIndex(
                name: "IX_AdminDisp_ApplicationUserId",
                table: "AdminDisp");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AdminDisp");
        }
    }
}
