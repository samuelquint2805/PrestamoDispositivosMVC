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
        public string Tipo { get; set; }
        public string Procesador { get; set; }
        public int Almacenamiento { get; set; }
        public string TarjetaGrafica { get; set; }

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
