using PrestamoDispositivos.Models;

namespace PrestamoDispositivos.DTO
{
    public class deviceManagerDTO
    {
        public Guid IdAdmin { get; set; }

        public string Nombre { get; set; }

        public string Usuario { get; set; }

        public string Contraseña { get; set; }

        //Relación con Prestamos
        public ICollection<Loan> Loans { get; set; }
    }
}
