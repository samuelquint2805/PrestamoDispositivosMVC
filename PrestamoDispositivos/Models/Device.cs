using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PrestamoDispositivos.Models
{
    public class Device
    {
        [Key]
        public int IdDisp { get; set; }
        public string Tipo { get; set; }
        public string Procesador { get; set; }
        public int Almacenamiento { get; set; }
        public string TarjetaGrafica { get; set; }
        public string EstadoDisp { get; set; }

        // 🔗 Relación 1 a muchos con Prestamos
        public ICollection<Loan> Prestamos { get; set; }
    }
}
