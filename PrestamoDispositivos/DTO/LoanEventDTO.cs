using PrestamoDispositivos.Models;

namespace PrestamoDispositivos.DTO
{
    public class LoanEventDTO
    {
        public Guid IdEvento { get; set; }
        public string TipoPrestamos { get; set; }

        //Relación con Loan
        public ICollection<Loan> EventosPrestamos { get; set; }
    }
}
