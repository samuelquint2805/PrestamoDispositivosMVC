using PrestamoDispositivos.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.DTO
{
    public class LoanDTO
    {

        public Guid IdPrestamos { get; set; }
       
        public DateTime FechaEvento { get; set; }
       
        public string? EstadoPrestamo { get; set; }

        //relacion a uno con Device
        
        public Guid? IdDispo { get; set; }
        public deviceDTO? Dispositivo { get; set; }

        //Relacion a uno con Request

        public Guid? IdUser { get; set; }
        
        public ApplicationUserDTO? User { get; set; }
    }
}
