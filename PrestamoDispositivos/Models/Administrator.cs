using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class Administrator
    {

        #region Atributos
        [Key]
        [Required]
        public Guid IdAdmin { get; set; }

        [Required(ErrorMessage = "El campo Nombre es requerido")]
        public required string Nombre { get; set; }
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
