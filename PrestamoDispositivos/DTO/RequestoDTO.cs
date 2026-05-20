using PrestamoDispositivos.Models;
using System.ComponentModel.DataAnnotations;

namespace PrestamoDispositivos.DTO
{
    public class RequestoDTO
    {
        #region Atributos       
        [Key]
        [Required]
        public Guid IdSolicitud { get; set; }

        
        public DateTime FechaSolicitud { get; set; }
        
        public DateTime FechaAprobacion { get; set; }
        
        public string? EstadoSolicitud { get; set; }
        #endregion

        // NuevoEstado: viene del botón submit con name="NuevoEstado" value="Aprobada|Rechazada"
        public string? NuevoEstado { get; set; }

        // Observacion: comentario opcional del prestamista al revisar
        public string? Observacion { get; set; }

        #region Relaciones
        //  Relación a uno con User
        public Guid? idUser { get; set; }
        public ApplicationUserDTO? User { get; set; }
        #endregion
    }
}
