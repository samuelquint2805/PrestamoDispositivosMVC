using PrestamoDispositivos.Models;
using System.ComponentModel.DataAnnotations;

namespace PrestamoDispositivos.DTO
{
    public class deviceDTO
    {
   
        public int IdDisp { get; set; }
        public string Tipo { get; set; }
        public string Procesador { get; set; }
        public int Almacenamiento { get; set; }
        public string TarjetaGrafica { get; set; }
        public string EstadoDisp { get; set; }

    
    }
}
