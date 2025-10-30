using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamoDispositivos.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionCardinalidadStudentydevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Prestamos_IdDispo",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_IdEstudiante",
                table: "Prestamos");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_IdDispo",
                table: "Prestamos",
                column: "IdDispo");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_IdEstudiante",
                table: "Prestamos",
                column: "IdEstudiante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Prestamos_IdDispo",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_IdEstudiante",
                table: "Prestamos");

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
        }
    }
}
