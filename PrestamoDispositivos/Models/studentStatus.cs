using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class studentStatus
    {
     
        public Guid IdStatus { get; set; }
       
        public string EstadoCuenta { get; set; }

        // Relación con Prestamos
        public ICollection<Loan> Prestamos { get; set; }
    }
}
