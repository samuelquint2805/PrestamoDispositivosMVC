using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamoDispositivos.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminDisp",
                columns: table => new
                {
                    IdAdmin = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contraseña = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminDisp", x => x.IdAdmin);
                });

            migrationBuilder.CreateTable(
                name: "Dispositivos",
                columns: table => new
                {
                    IdDisp = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Procesador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Almacenamiento = table.Column<int>(type: "int", nullable: false),
                    TarjetaGrafica = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoDisp = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositivos", x => x.IdDisp);
                });

            migrationBuilder.CreateTable(
                name: "EstadoEstudiantes",
                columns: table => new
                {
                    IdStatus = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstEstu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoEstudiantes", x => x.IdStatus);
                });

            migrationBuilder.CreateTable(
                name: "Estudiante",
                columns: table => new
                {
                    IdEst = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<int>(type: "int", nullable: false),
                    Edad = table.Column<int>(type: "int", nullable: false),
                    semestreCursado = table.Column<int>(type: "int", nullable: false),
                    Contraseña = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorreoIns = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Carnet = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiante", x => x.IdEst);
                });

            migrationBuilder.CreateTable(
                name: "EventoPrestamos",
                columns: table => new
                {
                    IdEvento = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoPrestamos = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoPrestamos", x => x.IdEvento);
                });

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

            migrationBuilder.CreateTable(
                name: "Prestamos",
                columns: table => new
                {
                    IdPrestamos = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoPrestamo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdEstudiante = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDispo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAdminDev = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdEvento = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestamos", x => x.IdPrestamos);
                    table.ForeignKey(
                        name: "FK_Prestamos_AdminDisp_IdAdminDev",
                        column: x => x.IdAdminDev,
                        principalTable: "AdminDisp",
                        principalColumn: "IdAdmin",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prestamos_Dispositivos_IdDispo",
                        column: x => x.IdDispo,
                        principalTable: "Dispositivos",
                        principalColumn: "IdDisp",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prestamos_Estudiante_IdEstudiante",
                        column: x => x.IdEstudiante,
                        principalTable: "Estudiante",
                        principalColumn: "IdEst",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prestamos_EventoPrestamos_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "EventoPrestamos",
                        principalColumn: "IdEvento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_IdAdminDev",
                table: "Prestamos",
                column: "IdAdminDev");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_IdDispo",
                table: "Prestamos",
                column: "IdDispo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_IdEstudiante",
                table: "Prestamos",
                column: "IdEstudiante",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_IdEvento",
                table: "Prestamos",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_StudentstudentStatus_EstadoEstudiante",
                table: "StudentstudentStatus",
                column: "EstadoEstudiante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prestamos");

            migrationBuilder.DropTable(
                name: "StudentstudentStatus");

            migrationBuilder.DropTable(
                name: "AdminDisp");

            migrationBuilder.DropTable(
                name: "Dispositivos");

            migrationBuilder.DropTable(
                name: "EventoPrestamos");

            migrationBuilder.DropTable(
                name: "EstadoEstudiantes");

            migrationBuilder.DropTable(
                name: "Estudiante");
        }
    }
}
