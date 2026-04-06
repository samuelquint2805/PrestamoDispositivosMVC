namespace PrestamoDispositivos.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string NombreRol { get; set; } = "";
        public string Estado { get; set; } = "";
        public DateTime FechaRegistro { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}
