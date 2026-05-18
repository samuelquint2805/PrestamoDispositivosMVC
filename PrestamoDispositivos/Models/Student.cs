using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class Student 
    {
        #region Atributos
        [Key]
        [Required]
        public Guid IdEst { get; set; }

        [Required(ErrorMessage = "El campo Nombre es requerido")]
        public required string Nombre { get; set; }
        [Required(ErrorMessage = "El campo Carnet es requerido")]
        public required int carnet { get; set; }
        [Required(ErrorMessage = "El campo Documento de identificación es requerido")]
        public required int DocumentoID { get; set; }
        [Required(ErrorMessage = "El campo Celular es requerido")]
        public required int numeroCelular { get; set; }


        #endregion

        #region Relaciones
        //Relacion a uno con ApplicationUser (Identity)
       
        public Guid? ApplicationUserId { get; set; }
        public ApplicationUser? User { get; set; }
        #endregion

    }
}
