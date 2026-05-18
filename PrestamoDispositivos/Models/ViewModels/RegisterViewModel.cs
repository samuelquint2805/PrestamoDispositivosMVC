using System.ComponentModel.DataAnnotations;

namespace PrestamoDispositivos.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 50 caracteres.")]
        [Display(Name = "Nombre de Usuario")]
        public string UserName { get; set; } = "";

        [Required(ErrorMessage = "El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Confirma tu contraseña.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; } = "";
    }
}