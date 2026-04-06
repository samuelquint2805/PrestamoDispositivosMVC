using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class Request
    {
        #region Atributos       
        [Key]
        [Required]
        public Guid IdSolicitud { get; set; }

        [Required(ErrorMessage = "El campo Fecha de la solicitud es requerido")]
      public DateTime FechaSolicitud { get; set; }
        [Required(ErrorMessage = "El campo Fecha de Aprobación es requerido")]
        public DateTime FechaAprobacion { get; set; }
        [Required(ErrorMessage = "El campo Estado de la Solicitud es requerido")]
        public string? EstadoSolicitud { get; set; }
        #endregion

      
        #region Relaciones
        //  Relación a uno con User
        public Guid? idUser { get; set; }
      
        public ApplicationUser? User { get; set; }

        #endregion
    }
}
