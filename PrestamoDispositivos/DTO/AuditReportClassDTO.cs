using PrestamoDispositivos.Models;

namespace PrestamoDispositivos.DTO
{
    public class AuditReportClassDTO
    {
        public Guid IdAudit { get; set; }
        public required string accion { get; set; }
        public required DateTime FechaEvento { get; set; }
        public required string Descripcion { get; set; }
        public virtual ICollection<ApplicationUserDTO> ReportUs { get; set; } = new List<ApplicationUserDTO>();
    }
}
