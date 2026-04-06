using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class setRol
    {
        #region Atributos

        [Key]
        [Required]
        public Guid idRol { get; set; }
        [Required(ErrorMessage = "Nombre del Rol es requerido")]
        public string? nombreRol { get; set; }
        [Required(ErrorMessage = "El campo de Descripción es requerido")]
        public string? descripcion { get; set; }
        [Required(ErrorMessage = "Los Permisos son requeridos")]
        public string? permisos { get; set; }
        #endregion
        #region Relaciones
        //Relacion hacia User, para asignacion de Rol
        //  Relación a uno con Prestamos
    
        public virtual ICollection<ApplicationUser> Usuarios { get; set; } = new List<ApplicationUser>();
        #endregion

    }
}
