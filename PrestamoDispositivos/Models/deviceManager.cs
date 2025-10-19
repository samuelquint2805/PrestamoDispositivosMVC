using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PrestamoDispositivos.Models
{
    public class deviceManager
    {
     
        public Guid IdAdmin { get; set; }
      
        public string Nombre { get; set; }
       
        public string Usuario { get; set; }
       
        public string Contraseña { get; set; }

        // Relación 1 a muchos con Prestamos
        public ICollection<Loan> Prestamos { get; set; }
    }
}
