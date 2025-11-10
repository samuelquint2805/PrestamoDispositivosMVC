using Microsoft.EntityFrameworkCore;
using PrestamoDispositivos.Models;
using System;

namespace PrestamoDispositivos.DataContext.Sections
{
    public class DatacontextPres : DbContext
    {
        public DatacontextPres(DbContextOptions<DatacontextPres> options)
           : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student <-> studentStatus (1:1) - FK in studentStatus.StudentId
            modelBuilder.Entity<Student>()
                .HasOne(s => s.EstadoEst)
                .WithOne(ss => ss.Student)
                .HasForeignKey<studentStatus>(ss => ss.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student <-> Loan (1:1) - FK in Loan.IdEstudiante
            modelBuilder.Entity<Loan>()
        .HasOne(l => l.Estudiante)
        .WithMany(s => s.Prestamos)
        .HasForeignKey(l => l.IdEstudiante)
        .OnDelete(DeleteBehavior.Cascade);

            // Student <-> ApplicationUser (1:1) - FK in Student.ApplicationUserId
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)

                .WithOne()
                .HasForeignKey<Student>(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // deviceManager -> Loan (1:N)
            modelBuilder.Entity<deviceManager>()
                .HasMany(dm => dm.Loans)
                .WithOne(l => l.DeviceManager)
                .HasForeignKey(l => l.IdAdminDev);

            // Device -> Loan (1:N)
            modelBuilder.Entity<Device>()
                .HasMany(d => d.Prestamos)
                .WithOne(l => l.Dispositivo)
                .HasForeignKey(l => l.IdDispo);
        }

        // NUEVO: tabla para usuarios ..clase ApplicationUser
        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<Student> Estudiante { get; set; }
        public DbSet<deviceManager> AdminDisp { get; set; }
        public DbSet<Device> Dispositivos { get; set; }
        public DbSet<Loan> Prestamos { get; set; }
        public DbSet<LoanEvent> EventoPrestamos { get; set; }
        public DbSet<studentStatus> EstadoEstudiantes { get; set; }
    }
}