using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamoDispositivos.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionCamposEstudiantes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EstadoEstudiantes_Estudiante_StudentId",
                table: "EstadoEstudiantes");

            migrationBuilder.DropIndex(
                name: "IX_EstadoEstudiantes_StudentId",
                table: "EstadoEstudiantes");

            migrationBuilder.RenameColumn(
                name: "EstadoEstudiante",
                table: "Estudiante",
                newName: "StudentId");

            migrationBuilder.AddColumn<Guid>(
                name: "EstadoEstId",
                table: "Estudiante",
                type: "uniqueidentifier",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estudiante_EstadoEstudiantes_StudentId",
                table: "Estudiante");

            migrationBuilder.DropIndex(
                name: "IX_Estudiante_StudentId",
                table: "Estudiante");

            migrationBuilder.DropColumn(
                name: "EstadoEstId",
                table: "Estudiante");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Estudiante",
                newName: "EstadoEstudiante");

            migrationBuilder.CreateIndex(
                name: "IX_EstadoEstudiantes_StudentId",
                table: "EstadoEstudiantes",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EstadoEstudiantes_Estudiante_StudentId",
                table: "EstadoEstudiantes",
                column: "StudentId",
                principalTable: "Estudiante",
                principalColumn: "IdEst",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
