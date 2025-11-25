using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class Student 
    {
        #region Atributos
        [Key]
        public Guid IdEst { get; set; }
        [Required(ErrorMessage = "El campo de Nombre es requerido")]
        public string Nombre { get; set; } = String.Empty;
        
        [Required(ErrorMessage = "El campo de Teléfono es requerido")]
        [MaxLength(50)]
        public string? Telefono { get; set; }
        [Required(ErrorMessage = "El campo de Edad es requerido")]
        public int Edad { get; set; }
        [Required(ErrorMessage = "El campo de Semestre Cursado es requerido")]
        public int semestreCursado { get; set; }
        
        [Required(ErrorMessage = "El campo de Correo Institucional es requerido")]
        
        public int Carnet { get; set; }

        #endregion


        // apartado para Relaciones con otras clases (tablas)
        #region Relaciones

        // Relación con ApplicationUser (Identity)
        [ForeignKey("User")]
        public Guid? ApplicationUserId { get; set; }
        public ApplicationUser? User { get; set; }

        //Relacion a uno con studenStatus
        [ForeignKey("EstadoEstudiante")]
        public Guid? EstadoEstId { get; set; }
        public studentStatus? EstadoEst { get; set; }

       
        // Relación a uno con Prestamos
        [InverseProperty("Estudiante")]
        public ICollection<Loan> Prestamos { get; set; } = new List<Loan>();
        #endregion
    }
}
