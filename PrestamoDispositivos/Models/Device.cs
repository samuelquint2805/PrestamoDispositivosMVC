using PrestamoDispositivos.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace PrestamoDispositivos.Models
{
    public class Device
    {

        #region Atributos       
        [Key]
        [Required]
        public Guid IdDisp { get; set; }
       
        [Required(ErrorMessage = "El campo de Serial es requerido")]
        public string? Serial { get; set; }
        [Required(ErrorMessage = "El campo de Marca es requerido")]
        public string? Marca { get; set; }
        [Required(ErrorMessage = "El campo de Especificaciones es requerido")]
        public string? Especificaciones { get; set; }
        [Required(ErrorMessage = "El campo de Estado del Dispositivo es requerido")]
        public  string? EstadoEquipo { get; set; }
        [Required(ErrorMessage = "El campo de Imagen del Dispositivo es requerido")]
        [MaxLength(500)]
        public string? URLImagen { get; set; }
        #endregion

        // apartado para Relaciones con otras clases (tablas)
        #region Relaciones
        //  Relación a uno con Prestamos
        [InverseProperty("Dispositivo")]
        public ICollection<Loan> Prestamos { get; set; } = new List<Loan>();
        #endregion
    }
}
