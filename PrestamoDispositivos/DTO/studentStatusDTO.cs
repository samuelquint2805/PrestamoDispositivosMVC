using PrestamoDispositivos.Models;

namespace PrestamoDispositivos.DTO
{
    public class studentStatusDTO
    {
        public Guid IdStatus { get; set; }

        public required string EstEstu { get; set; }

        // Relación con Student
        public ICollection<StudentDTO>? Estudiante { get; set; }
    }
}
