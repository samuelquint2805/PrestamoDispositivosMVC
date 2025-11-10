using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamoDispositivos.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentstudentStatus");

            migrationBuilder.DropColumn(
                name: "Contraseña",
                table: "Estudiante");

            migrationBuilder.DropColumn(
                name: "CorreoIns",
                table: "Estudiante");

            migrationBuilder.DropColumn(
                name: "Usuario",
                table: "Estudiante");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "Estudiante",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EstadoEstudiante",
                table: "Estudiante",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "EstadoEstudiantes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorCodeExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    LockoutEnd = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estudiante_ApplicationUserId",
                table: "Estudiante",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EstadoEstudiantes_StudentId",
                table: "EstadoEstudiantes",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ApplicationUserId",
                table: "Users",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EstadoEstudiantes_Estudiante_StudentId",
                table: "EstadoEstudiantes",
                column: "StudentId",
                principalTable: "Estudiante",
                principalColumn: "IdEst",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Estudiante_Users_ApplicationUserId",
                table: "Estudiante",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EstadoEstudiantes_Estudiante_StudentId",
                table: "EstadoEstudiantes");

            migrationBuilder.DropForeignKey(
                name: "FK_Estudiante_Users_ApplicationUserId",
                table: "Estudiante");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Estudiante_ApplicationUserId",
                table: "Estudiante");

            migrationBuilder.DropIndex(
                name: "IX_EstadoEstudiantes_StudentId",
                table: "EstadoEstudiantes");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Estudiante");

            migrationBuilder.DropColumn(
                name: "EstadoEstudiante",
                table: "Estudiante");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "EstadoEstudiantes");

            migrationBuilder.AddColumn<string>(
                name: "Contraseña",
                table: "Estudiante",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CorreoIns",
                table: "Estudiante",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Usuario",
                table: "Estudiante",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StudentstudentStatus",
                columns: table => new
                {
                    EstadoEstIdStatus = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstadoEstudiante = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentstudentStatus", x => new { x.EstadoEstIdStatus, x.EstadoEstudiante });
                    table.ForeignKey(
                        name: "FK_StudentstudentStatus_EstadoEstudiantes_EstadoEstIdStatus",
                        column: x => x.EstadoEstIdStatus,
                        principalTable: "EstadoEstudiantes",
                        principalColumn: "IdStatus",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentstudentStatus_Estudiante_EstadoEstudiante",
                        column: x => x.EstadoEstudiante,
                        principalTable: "Estudiante",
                        principalColumn: "IdEst",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentstudentStatus_EstadoEstudiante",
                table: "StudentstudentStatus",
                column: "EstadoEstudiante");
        }
    }
}
