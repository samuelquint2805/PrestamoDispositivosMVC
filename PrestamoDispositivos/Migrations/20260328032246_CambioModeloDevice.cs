using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamoDispositivos.Migrations
{
    /// <inheritdoc />
    public partial class CambioModeloDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Serial",
                table: "Dispositivos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        
       
    }
}
