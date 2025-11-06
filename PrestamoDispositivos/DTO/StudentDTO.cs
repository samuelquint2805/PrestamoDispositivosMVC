using PrestamoDispositivos.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.DTO
{
    public class StudentDTO
    {
        public Guid IdEst { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Telefono { get; set; }
        public int Edad { get; set; }
        public int semestreCursado { get; set; }
        public string Contraseña { get; set; }
        public string CorreoIns { get; set; }
        public int Carnet { get; set; }


        // Relación con studentStatus
        public ICollection<studentStatusDTO> EstadoEst { get; set; }

        // Relación con Prestamos
        public ICollection<LoanDTO> Prestamos { get; set; } = new List<LoanDTO>();
    }
}
