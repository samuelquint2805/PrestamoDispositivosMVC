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
        public Guid IdDisp { get; set; }
        [Required(ErrorMessage = "El campo de Tipo es requerido")]
        public string Tipo { get; set; }
        [Required(ErrorMessage = "El campo de Procesador es requerido")]
        public string Procesador { get; set; }
        [Required(ErrorMessage = "El campo de Almacenamiento es requerido")]
        public int Almacenamiento { get; set; }
        [Required(ErrorMessage = "El campo de TarjetaGráfica es requerido")]
        public string TarjetaGrafica { get; set; }
        [Required(ErrorMessage = "El campo de Estado del Dispositivo es requerido")]
        public string EstadoDisp { get; set; }
        #endregion

        // apartado para Relaciones con otras clases (tablas)
        #region Relaciones
        //  Relación a uno con Prestamos
        [InverseProperty("Dispositivo")]
        public ICollection<Loan> Prestamos { get; set; } = new List<Loan>();
        #endregion
    }
}
