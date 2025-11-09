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
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo de Usuario es requerido")]
        public string Usuario { get; set; }
        [Required(ErrorMessage = "El campo de Contraseña es requerido")]

        public string Contraseña { get; set; }
        #endregion

        #region Relaciones
        // Relación a muchos con Prestamos
        
        public ICollection<Loan> Loans { get; set; }
        #endregion
    }
}
