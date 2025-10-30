using System.ComponentModel.DataAnnotations;

namespace PrestamoDispositivos.Data.Entities
{
    public class Sections
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Sección")]
        [Required(ErrorMessage = " El campo {0} es requerido")]
        public required string Name { get; set; }

        [Display(Name = "Descripción")]
        public string? Descripción { get; set; }

        public bool ishidden { get; set; }
    }
}
