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
            migrationBuilder.AddColumn<string>(
             name: "Serial",
             table: "Dispositivos",
             nullable: true);
        }

     
    }
}
