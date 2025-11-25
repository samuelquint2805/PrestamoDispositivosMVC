using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace PrestamoDispositivos.Models
{
    public class deviceManager 
    {


        #region Atributos
        [Key]
        public Guid IdAdmin { get; set; }

        [Required(ErrorMessage = "El campo Nombre es requerido")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El campo de Usuario es requerido")]
        public required string Usuario { get; set; }
        [Required(ErrorMessage = "El campo de Contraseña es requerido")]

        public required string Contraseña { get; set; }
        #endregion

        #region Relaciones
        // Relación a muchos con Prestamos
        
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();

        //Relacion a uno con ApplicationUser (Identity)
        [ForeignKey("User")]
        public Guid? ApplicationUserId { get; set; }
        public ApplicationUser? User { get; set; }
        #endregion
    }
}
