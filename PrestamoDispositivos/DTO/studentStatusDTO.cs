using PrestamoDispositivos.Models;

namespace PrestamoDispositivos.DTO
{
    public class studentStatusDTO
    {
        public Guid IdStatus { get; set; }

        public string EstEstu { get; set; }

        // Relación con Student
        public ICollection<StudentDTO> Prestamos { get; set; }
    }
}
