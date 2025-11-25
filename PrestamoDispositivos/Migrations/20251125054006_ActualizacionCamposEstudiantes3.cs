using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamoDispositivos.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionCamposEstudiantes3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estudiante_EstadoEstudiantes_StudentId",
                table: "Estudiante");

            migrationBuilder.DropIndex(
                name: "IX_Estudiante_StudentId",
                table: "Estudiante");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Estudiante");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "EstadoEstudiantes");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiante_EstadoEstId",
                table: "Estudiante",
                column: "EstadoEstId");

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiante_EstadoEstudiantes_EstadoEstId",
                table: "Estudiante",
                column: "EstadoEstId",
                principalTable: "EstadoEstudiantes",
                principalColumn: "IdStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estudiante_EstadoEstudiantes_EstadoEstId",
                table: "Estudiante");

            migrationBuilder.DropIndex(
                name: "IX_Estudiante_EstadoEstId",
                table: "Estudiante");

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "Estudiante",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "EstadoEstudiantes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Estudiante_StudentId",
                table: "Estudiante",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiante_EstadoEstudiantes_StudentId",
                table: "Estudiante",
                column: "StudentId",
                principalTable: "EstadoEstudiantes",
                principalColumn: "IdStatus");
        }
    }
}
