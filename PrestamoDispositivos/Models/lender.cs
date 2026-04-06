using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class lender
    {
        #region Atributos
        [Key]
        [Required]
        public Guid idPres { get; set; }

        [Required(ErrorMessage = "El campo Nombre es requerido")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "El campo Celular es requerido")]
        public int? numeroCelular { get; set; }


        #endregion

        #region Relaciones
        //Relacion a uno con ApplicationUser (Identity)

        public Guid? ApplicationUserId { get; set; }
      
        public ApplicationUser? User { get; set; }
        #endregion
    }
}
