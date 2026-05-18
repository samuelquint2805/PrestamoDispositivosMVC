using PrestamoDispositivos.Models;

namespace PrestamoDispositivos.DTO
{
    public class lenderDTO
    {
        public Guid idPres { get; set; }
        public required string Nombre { get; set; }
        public required int numeroCelular { get; set; }

        public Guid? ApplicationUserId { get; set; }
        public ApplicationUserDTO? User { get; set; }
    }
}
