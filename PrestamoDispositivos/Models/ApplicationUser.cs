using PrestamoDispositivos.Services.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    // Clase de usuario para autenticación manual (no Identity)
    public class ApplicationUser 
    {
        [Key]
        [Required]
       public Guid idUsuario { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "El campo Usuario es requerido")]
        public string? usuario { get; set; }
        [Required(ErrorMessage = "El campo Correo es requerido")]
        public string? CorreoElectrónico { get; set; }
        [Required(ErrorMessage = "El campo Contraseña es requerido")]
        public string? PasswordHash { get; set; }
        [Required(ErrorMessage = "El campo Estado es requerido")]
        public string? Estado { get; set; }
        [Required(ErrorMessage = "La fecha del registro es requerido")]
        public DateTime fechaRegistro { get; set; }
         public string? codigo2FA { get; set; }



        #region Relaciones
        public Guid? idSuperior { get; set; }

        // Propiedad de navegación hacia el superior
        [ForeignKey("idSuperior")]
        public virtual ApplicationUser? Superior { get; set; }

        //Relacion hacia Estudiantes
        //public Guid? StudentUser   { get; set; }
        //[ForeignKey("StudentUser")]
        public  virtual Student? studentUsuario { get; set; }

        //Relacion hacia Prestamista (Lender)
        //public Guid? LenderUser   { get; set; }
        //[ForeignKey("LenderUser")]
        public virtual lender? LenderUsuario  { get; set; }

        //Relacion hacia Estudiantes
        //public Guid? AdminUser   { get; set; }
        //[ForeignKey("AdminUser")]
        public virtual Administrator? AdminUsuario{ get; set; }

        //Relacion hacia Estudiantes
        public virtual Guid? RolUser{ get; set; }
     
        public setRol? Rol { get; set; }    

       
        public virtual ICollection<Request> Solicitudes { get; set; } = new List<Request>();

       
        public virtual ICollection<Loan> PrestamosUser { get; set; } = new List<Loan>();

       
        public virtual ICollection<AuditReportsClass> ReporAudit { get; set; } = new List<AuditReportsClass>();

        #endregion
    }
}