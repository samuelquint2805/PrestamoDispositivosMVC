using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamoDispositivos.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dispositivos",
                columns: table => new
                {
                    IdDisp = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Serial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Especificaciones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoEquipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    URLImagen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositivos", x => x.IdDisp);
                });

            migrationBuilder.CreateTable(
                name: "ReportesyAuditorias",
                columns: table => new
                {
                    IdAudit = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesyAuditorias", x => x.IdAudit);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    idRol = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    nombreRol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    permisos = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.idRol);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    idUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    usuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorreoElectrónico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    codigo2FA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    idSuperior = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RolUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.idUsuario);
                    table.ForeignKey(
                        name: "FK_Users_Rol_RolUser",
                        column: x => x.RolUser,
                        principalTable: "Rol",
                        principalColumn: "idRol",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Users_idSuperior",
                        column: x => x.idSuperior,
                        principalTable: "Users",
                        principalColumn: "idUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    IdAdmin = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numeroCelular = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.IdAdmin);
                    table.ForeignKey(
                        name: "FK_Administradores_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "idUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Estudiante",
                columns: table => new
                {
                    IdEst = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    carnet = table.Column<int>(type: "int", nullable: false),
                    DocumentoID = table.Column<int>(type: "int", nullable: false),
                    numeroCelular = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiante", x => x.IdEst);
                    table.ForeignKey(
                        name: "FK_Estudiante_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "idUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Prestamista",
                columns: table => new
                {
                    idPres = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numeroCelular = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestamista", x => x.idPres);
                    table.ForeignKey(
                        name: "FK_Prestamista_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "idUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Prestamos",
                columns: table => new
                {
                    IdPrestamos = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoPrestamo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdDispo = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestamos", x => x.IdPrestamos);
                    table.ForeignKey(
                        name: "FK_Prestamos_Dispositivos_IdDispo",
                        column: x => x.IdDispo,
                        principalTable: "Dispositivos",
                        principalColumn: "IdDisp",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prestamos_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "idUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "solicitud",
                columns: table => new
                {
                    IdSolicitud = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoSolicitud = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    idUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_solicitud", x => x.IdSolicitud);
                    table.ForeignKey(
                        name: "FK_solicitud_Users_idUser",
                        column: x => x.idUser,
                        principalTable: "Users",
                        principalColumn: "idUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAuditReports",
                columns: table => new
                {
                    ReporAuditIdAudit = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportUsidUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAuditReports", x => new { x.ReporAuditIdAudit, x.ReportUsidUsuario });
                    table.ForeignKey(
                        name: "FK_UserAuditReports_ReportesyAuditorias_ReporAuditIdAudit",
                        column: x => x.ReporAuditIdAudit,
                        principalTable: "ReportesyAuditorias",
                        principalColumn: "IdAudit",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAuditReports_Users_ReportUsidUsuario",
                        column: x => x.ReportUsidUsuario,
                        principalTable: "Users",
                        principalColumn: "idUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administradores_ApplicationUserId",
                table: "Administradores",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiante_ApplicationUserId",
                table: "Estudiante",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamista_ApplicationUserId",
                table: "Prestamista",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_IdDispo",
                table: "Prestamos",
                column: "IdDispo");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_IdUser",
                table: "Prestamos",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_solicitud_idUser",
                table: "solicitud",
                column: "idUser");

            migrationBuilder.CreateIndex(
                name: "IX_UserAuditReports_ReportUsidUsuario",
                table: "UserAuditReports",
                column: "ReportUsidUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Users_idSuperior",
                table: "Users",
                column: "idSuperior");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RolUser",
                table: "Users",
                column: "RolUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "Estudiante");

            migrationBuilder.DropTable(
                name: "Prestamista");

            migrationBuilder.DropTable(
                name: "Prestamos");

            migrationBuilder.DropTable(
                name: "solicitud");

            migrationBuilder.DropTable(
                name: "UserAuditReports");

            migrationBuilder.DropTable(
                name: "Dispositivos");

            migrationBuilder.DropTable(
                name: "ReportesyAuditorias");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
