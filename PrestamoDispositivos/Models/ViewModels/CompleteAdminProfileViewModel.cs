using System.ComponentModel.DataAnnotations;

namespace PrestamoDispositivos.Models.ViewModels
{
    public class CompleteAdminProfileViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El número de celular es requerido.")]
        [Display(Name = "Número de Celular")]
        public int NumeroCelular { get; set; }  

    }
}
