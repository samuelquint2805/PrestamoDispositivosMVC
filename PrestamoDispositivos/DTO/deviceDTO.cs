using PrestamoDispositivos.Models;
using System.ComponentModel.DataAnnotations;

namespace PrestamoDispositivos.DTO
{
    public class deviceDTO
    {
        [Required]
        public Guid IdDisp { get; set; }
        
        public required string Tipo { get; set; }
        
        public required string Procesador { get; set; }
        
        public required int Almacenamiento { get; set; }
        
        public required string TarjetaGrafica { get; set; }
        
        public required string EstadoDisp { get; set; }

        //relacion hacia Loan
        public ICollection<LoanDTO> Prestamos { get; set; } = new List<LoanDTO>();

    }
}
