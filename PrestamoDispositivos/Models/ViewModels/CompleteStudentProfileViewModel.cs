using System.ComponentModel.DataAnnotations;

namespace PrestamoDispositivos.Models.ViewModels
{
    public class CompleteStudentProfileViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [StringLength(50, ErrorMessage = "El teléfono no puede exceder los 50 caracteres")]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La edad es requerida")]
        [Range(15, 100, ErrorMessage = "La edad debe estar entre 15 y 100 años")]
        [Display(Name = "Edad")]
        public int Edad { get; set; }

        [Required(ErrorMessage = "El semestre cursado es requerido")]
        [Range(1, 12, ErrorMessage = "El semestre debe estar entre 1 y 12")]
        [Display(Name = "Semestre Cursado")]
        public int SemestreCursado { get; set; }

        [Required(ErrorMessage = "El carnet es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El carnet debe ser un número válido")]
        [Display(Name = "Carnet Estudiantil")]
        public int Carnet { get; set; }
    }
}
