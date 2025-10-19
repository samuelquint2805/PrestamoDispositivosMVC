using PrestamoDispositivos.Models;

namespace PrestamoDispositivos.DTO
{
    public class LoanDTO
    {
        public Guid IdPrestamos { get; set; }

        // Llaves foráneas

        public Guid IdEst { get; set; }
        public Student Estudiante { get; set; }

        public Guid IdDispositivo { get; set; }
        public Device Dispositivo { get; set; }

        public Guid IdAdminDis { get; set; }
        public deviceManager AdminDisp { get; set; }

        public Guid EstadoPrestamos { get; set; }
        public LoanEvent EventoPrestamos { get; set; }
    }
}
