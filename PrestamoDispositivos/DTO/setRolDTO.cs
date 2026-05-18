using PrestamoDispositivos.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.DTO
{
    public class setRolDTO
    {
        public Guid idRol { get; set; }
       
        public string? nombreRol { get; set; }
       
        public string? descripcion { get; set; }
        
        public string? permisos { get; set; }

       
        public virtual ICollection<ApplicationUserDTO> Usuarios { get; set; } = new List<ApplicationUserDTO>();
    }
}
