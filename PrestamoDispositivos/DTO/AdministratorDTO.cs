using PrestamoDispositivos.Models;

namespace PrestamoDispositivos.DTO
{
    public class AdministratorDTO
    {
        public required Guid IdAdmin { get; set; }
        public required string Nombre { get; set; }
        public required int numeroCelular { get; set; }

        #region Relaciones
        public Guid? ApplicationUserId { get; set; }
       public ApplicationUserDTO? User { get; set; }
        #endregion
    }
}
