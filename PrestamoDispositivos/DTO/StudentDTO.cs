
using PrestamoDispositivos.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.DTO
{
    public class StudentDTO
    {
        public Guid IdEst { get; set; }

   
        public required string Nombre { get; set; }
      
        public required int carnet { get; set; }
        
        public required int DocumentoID { get; set; }
        public required int numeroCelular { get; set; }



      
        public Guid? ApplicationUserId { get; set; }
        public ApplicationUserDTO? User { get; set; }
    }
}
