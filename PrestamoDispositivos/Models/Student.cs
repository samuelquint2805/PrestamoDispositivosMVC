using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class Student
    {
        public Guid IdEst { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public int Telefono { get; set; }
        public int Edad { get; set; }
        public int semestreCursado { get; set; }
        public string Contraseña { get; set; }
        public string CorreoIns { get; set; }
        public int Carnet { get; set; }
        public string EstadoCuenta { get; set; }

        // Relación con Prestamos
        public ICollection<Loan> Prestamos { get; set; }
    }
}
