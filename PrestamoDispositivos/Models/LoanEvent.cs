using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class LoanEvent
    {

        #region Atributos
        [Key]
        public Guid IdEvento { get; set; }
        public string TipoPrestamos { get; set; }
        #endregion

        // apartado para Relaciones con otras clases (tablas)
        #region Relaciones
        //  Relación a muchos con Prestamos
        [InverseProperty("EventoPrestamos")]
        public ICollection<Loan> EventosPrestamos { get; set; }
        #endregion
    }
}
