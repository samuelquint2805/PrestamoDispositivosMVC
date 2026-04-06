                                                               using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrestamoDispositivos.Models
{
    public class Loan
    {

        #region Atributos
        [Key]
        [Required]
        public Guid IdPrestamos { get; set; }
        [Required(ErrorMessage = "La fecha del evento es requerida")]
        public DateTime FechaEvento { get; set; }
        [Required(ErrorMessage = "El estado del préstamo es requerido")]
        public string? EstadoPrestamo { get; set; }
        #endregion

        // apartado para Relaciones con otras clases (tablas)
        #region relaciones
        

        //relacion a uno con Device
        [ForeignKey("Dispositivo")]
        public Guid? IdDispo { get; set; }
        public Device? Dispositivo { get; set; }

        //Relacion a uno con Request

        public Guid? IdUser { get; set; }

    
        public ApplicationUser? User { get; set; }             


        #endregion
    }
}
