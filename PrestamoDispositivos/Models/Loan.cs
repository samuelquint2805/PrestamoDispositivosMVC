using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class Loan
    {

        #region Atributos
        [Key]
        public Guid IdPrestamos { get; set; }
        [Required(ErrorMessage = "La fecha del evento es requerida")]
        public DateTime FechaEvento { get; set; }
        [Required(ErrorMessage = "El estado del préstamo es requerido")]
        public string? EstadoPrestamo { get; set; }
        #endregion

        // apartado para Relaciones con otras clases (tablas)
        #region relaciones
        //relacion a uno con Student
        [ForeignKey("Estudiante")]
        public Guid IdEstudiante { get; set; }
        public Student? Estudiante { get; set; }

        //relacion a uno con Device
        [ForeignKey("Dispositivo")]
        public Guid IdDispo { get; set; }
        public Device? Dispositivo { get; set; }

        //relacion a muchos con deviceManager
        [ForeignKey("DeviceManager")]
        public Guid IdAdminDev { get; set; }
        public deviceManager? DeviceManager { get; set; }
        

        //relacion uno a uno con LoanEvent
        [ForeignKey("EventoPrestamos")]
        public Guid IdEvento { get; set; }
        public LoanEvent? EventoPrestamos { get; set; }
        #endregion
    }
}
