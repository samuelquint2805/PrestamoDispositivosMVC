using System.ComponentModel.DataAnnotations;

namespace PrestamoDispositivos.Models
{
    public class LoanEvent
    {
   
        public Guid IdEvento { get; set; }
        public string TipoPrestamos { get; set; }      
        public DateTime FechaEvento { get; set; }

        //  Relación con Prestamos
        public ICollection<Loan> EventosPrestamos { get; set; }
    }
}
