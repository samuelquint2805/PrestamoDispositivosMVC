using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class Student
    {
        #region Atributos
        [Key]
        public Guid IdEst { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public int Telefono { get; set; }
        public int Edad { get; set; }
        public int semestreCursado { get; set; }
        public string Contraseña { get; set; }
        public string CorreoIns { get; set; }
        public int Carnet { get; set; }

        #endregion

        // apartado para Relaciones con otras clases (tablas)
        #region Relaciones
        //Relacion a uno con studenStatus
        [ForeignKey("EstadoEstudiante")]
        public ICollection<studentStatus> EstadoEst { get; set; }

        // Relación a uno con Prestamos
        [InverseProperty("Estudiante")]
        public ICollection<Loan> Prestamos { get; set; } = new List<Loan>();
        #endregion
    }
}
