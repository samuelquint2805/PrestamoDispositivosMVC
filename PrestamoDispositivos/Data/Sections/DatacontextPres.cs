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



            // Student <-> AppUser (1:1) 
            modelBuilder.Entity<Student>()
            .HasOne(s => s.User)
            .WithOne(u => u.studentUsuario)
            .HasForeignKey<Student>(s => s.ApplicationUserId) 
            .OnDelete(DeleteBehavior.ClientSetNull);

            // Lender <-> AppUser (1:1) 
            modelBuilder.Entity<lender>()
                .HasOne(l => l.User)
                .WithOne(u => u.LenderUsuario)
                .HasForeignKey<lender>(l => l.ApplicationUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Administrator <-> AppUser (1:1)
            modelBuilder.Entity<Administrator>()
                .HasOne(a => a.User)
                .WithOne(u => u.AdminUsuario)
                .HasForeignKey<Administrator>(a => a.ApplicationUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // setRol <-> AppUser (1:N) 
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(r => r.RolUser)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser <-> Request (1:N)
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(r => r.Solicitudes)
                .WithOne(u => u.User)
                .HasForeignKey(r => r.idUser)
                .OnDelete(DeleteBehavior.Cascade);

            // loan <-> AppUser (1:N)
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.User)                 
                .WithMany(u => u.PrestamosUser)      
                .HasForeignKey(l => l.IdUser)        
                .OnDelete(DeleteBehavior.Cascade);

            // AuditReportsClass <-> AppUser (1:N)
            modelBuilder.Entity<AuditReportsClass>()
                .HasMany(a => a.ReportUs)
                .WithMany(u => u.ReporAudit)
                .UsingEntity(j => j.ToTable("UserAuditReports"));

            //Loan <-> Device (N:1)
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Dispositivo)
                .WithMany(d => d.Prestamos)
                .HasForeignKey(l => l.IdDispo)
                .OnDelete(DeleteBehavior.Cascade);


        }

        // NUEVO: tabla para usuarios ..clase ApplicationUser
        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<Student> Estudiante { get; set; }
        public DbSet<Administrator> Administradores { get; set; }
        public DbSet<Device> Dispositivos { get; set; }
        public DbSet<Loan> Prestamos { get; set; }
        public DbSet<AuditReportsClass> ReportesyAuditorias{ get; set; }
        public DbSet<lender> Prestamista { get; set; }
        public DbSet<Request> solicitud{ get; set; }
        public DbSet<setRol> Rol{ get; set; }


    }
}