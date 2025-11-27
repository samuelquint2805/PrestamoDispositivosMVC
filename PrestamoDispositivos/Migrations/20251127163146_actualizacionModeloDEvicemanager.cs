using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamoDispositivos.Migrations
{
    /// <inheritdoc />
    public partial class actualizacionModeloDEvicemanager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_AdminDisp_IdAdminDev",
                table: "Prestamos");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Dispositivos_IdDispo",
                table: "Prestamos");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_EventoPrestamos_IdEvento",
                table: "Prestamos");

            migrationBuilder.DropColumn(
                name: "Contraseña",
                table: "AdminDisp");

            migrationBuilder.DropColumn(
                name: "Usuario",
                table: "AdminDisp");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdEvento",
                table: "Prestamos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdEstudiante",
                table: "Prestamos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdDispo",
                table: "Prestamos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdAdminDev",
                table: "Prestamos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_AdminDisp_IdAdminDev",
                table: "Prestamos",
                column: "IdAdminDev",
                principalTable: "AdminDisp",
                principalColumn: "IdAdmin");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Dispositivos_IdDispo",
                table: "Prestamos",
                column: "IdDispo",
                principalTable: "Dispositivos",
                principalColumn: "IdDisp");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_EventoPrestamos_IdEvento",
                table: "Prestamos",
                column: "IdEvento",
                principalTable: "EventoPrestamos",
                principalColumn: "IdEvento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_AdminDisp_IdAdminDev",
                table: "Prestamos");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Dispositivos_IdDispo",
                table: "Prestamos");

            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_EventoPrestamos_IdEvento",
                table: "Prestamos");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdEvento",
                table: "Prestamos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdEstudiante",
                table: "Prestamos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdDispo",
                table: "Prestamos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdAdminDev",
                table: "Prestamos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contraseña",
                table: "AdminDisp",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Usuario",
                table: "AdminDisp",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_AdminDisp_IdAdminDev",
                table: "Prestamos",
                column: "IdAdminDev",
                principalTable: "AdminDisp",
                principalColumn: "IdAdmin",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Dispositivos_IdDispo",
                table: "Prestamos",
                column: "IdDispo",
                principalTable: "Dispositivos",
                principalColumn: "IdDisp",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_EventoPrestamos_IdEvento",
                table: "Prestamos",
                column: "IdEvento",
                principalTable: "EventoPrestamos",
                principalColumn: "IdEvento",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
