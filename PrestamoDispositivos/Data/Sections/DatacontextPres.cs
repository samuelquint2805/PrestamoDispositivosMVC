using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Models;

namespace PrestamoDispositivos.DataContext.Sections
{
    public class DatacontextPres : DbContext
    {
        public DatacontextPres(DbContextOptions<DatacontextPres> options)
           : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder){}

        public DbSet<Student> Estudiante { get; set; }
        public DbSet<deviceManager> AdminDisp { get; set; }
        public DbSet<Device> Dispositivos { get; set; }
        public DbSet<Loan> Prestamos { get; set; }
        public DbSet<LoanEvent> EventoPrestamos { get; set; }
        public DbSet<studentStatus> EstadoEstudiantes { get; set; }
    }
}
