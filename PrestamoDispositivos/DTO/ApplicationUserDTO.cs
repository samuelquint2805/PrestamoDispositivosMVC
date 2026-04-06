using PrestamoDispositivos.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.DTO
{
    public class ApplicationUserDTO
    {
        public Guid idUsuario
        {
            get; set;
        }
        public required string? usuario { get; set; }
        public required string? CorreoElectrónico { get; set; }
        public required string? PasswordHash { get; set; }
        public required string? Estado { get; set; }
        public required DateTime fechaRegistro { get; set; }
        public required string? codigo2FA { get; set; }



        #region Relaciones
        public Guid? idSuperior { get; set; }

        // Propiedad de navegación hacia el superior
        public virtual ApplicationUserDTO? Superior { get; set; }

        //Relacion hacia Estudiantes
        public Guid? StudentUser { get; set; } 
        public StudentDTO? studentUsuario { get; set; }

        //Relacion hacia Prestamista (Lender)
        public Guid? LenderUser { get; set; }
        public lenderDTO? LenderUsuario { get; set; }

        //Relacion hacia Estudiantes
        public Guid? AdminUser { get; set; }
        public AdministratorDTO? AdminUsuario { get; set; }

        //Relacion hacia Estudiantes
        public Guid? RolUser { get; set; }
        public setRolDTO? Rol { get; set; }

        public virtual ICollection<RequestoDTO> Solicitudes { get; set; } = new List<RequestoDTO>();
        public virtual ICollection<LoanDTO> PrestamosUser { get; set; } = new List<LoanDTO>();
        public virtual ICollection<AuditReportClassDTO> ReporAudit { get; set; } = new List<AuditReportClassDTO>();
        #endregion
    }
}
