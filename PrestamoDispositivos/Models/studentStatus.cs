using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class studentStatus
    {
        [Key]
        public Guid IdStatus { get; set; }

        [Required(ErrorMessage = "El estado del estudiante es requerido")]
        public string EstEstu { get; set; } = string.Empty;

        //  Clave foránea hacia Student
        public Guid StudentId { get; set; }

        //  Relación 1:1 con Student
        [ForeignKey("StudentId")]
        public ICollection<Student> studentsStu { get; set; } 
    }
}

