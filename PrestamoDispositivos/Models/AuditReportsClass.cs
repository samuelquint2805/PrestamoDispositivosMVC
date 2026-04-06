using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class AuditReportsClass
    {
        [Key]
        [Required]
        public Guid? IdAudit { get; set; }
        [Required(ErrorMessage = "el campo acción es requerido")]
        public string? accion { get; set; }
        [Required(ErrorMessage = "La fecha del evento es requerida")]
        public DateTime? FechaEvento { get; set; }
        [Required(ErrorMessage = "La descripción es requerido")]
        public string? Descripcion { get; set; }


        
        public virtual ICollection<ApplicationUser> ReportUs { get; set; } = new List<ApplicationUser>();
    }
}
