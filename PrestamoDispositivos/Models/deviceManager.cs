using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace PrestamoDispositivos.Models
{
    public class deviceManager
    {

        #region Atributos
        [Key]
        public Guid IdAdmin { get; set; }

        public string Nombre { get; set; }

        public string Usuario { get; set; }

        public string Contraseña { get; set; }
        #endregion

        #region Relaciones
        // Relación a muchos con Prestamos
        
        public ICollection<Loan> Loans { get; set; }
        #endregion
    }
}
