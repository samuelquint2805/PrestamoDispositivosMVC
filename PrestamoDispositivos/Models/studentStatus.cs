using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class studentStatus
    {

        #region Atributos
        [Key]
        public Guid IdStatus { get; set; }
        [Required(ErrorMessage = "El estado del estudiante es requerido")]
        public string EstEstu { get; set; }
        #endregion

        // apartado para Relaciones con otras clases (tablas)
        #region Relaciones
        // Relación a muchos con Estudiantes
        public ICollection<Student> Prestamos { get; set; }
        #endregion
    }
}
