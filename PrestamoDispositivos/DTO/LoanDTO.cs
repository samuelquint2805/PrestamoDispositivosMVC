using PrestamoDispositivos.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.DTO
{
    public class LoanDTO
    {

        public Guid IdPrestamos { get; set; }

        public DateTime FechaEvento { get; set; }
        public required string EstadoPrestamo { get; set; }

        // Llaves foráneas

        public Guid  IdEstudiante { get; set; }
        public StudentDTO? Estudiante { get; set; }

        //relacion a uno con Device
       
        public Guid? IdDispo { get; set; }
        public deviceDTO?  Dispositivo { get; set; }

        //relacion a muchos con deviceManager
       
        public Guid? IdAdminDev { get; set; }
        public deviceManagerDTO? DeviceManager { get; set; }


        //relacion uno a uno con LoanEvent
       
        public Guid? IdEvento { get; set; }
        public LoanEventDTO? EventoPrestamos { get; set; }
    }
}
