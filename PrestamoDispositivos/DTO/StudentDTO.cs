
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.DTO
{
    public class StudentDTO
    {
        public Guid IdEst { get; set; }
        public required string Nombre { get; set; }
        public required string Telefono { get; set; }
        public int Edad { get; set; }
        public int semestreCursado { get; set; }
        public int Carnet { get; set; }


        // Relación con studentStatus

        public Guid? ApplicationUserId { get; set; }
        public ApplicationUserDTO? User { get; set; }

        public Guid? EstadoEstId { get; set; }
        public studentStatusDTO EstadoEst { get; set; }

        // Relación con Prestamos
        public ICollection<LoanDTO> Prestamos { get; set; } = new List<LoanDTO>();
    }
}
