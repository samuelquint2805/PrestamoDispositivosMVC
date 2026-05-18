


namespace PrestamoDispositivos.DTO
{
    public class deviceDTO
    {
        
        public Guid IdDisp { get; set; }
        public required string Serial { get; set; }
        public required string Marca { get; set; }
        public required string Especificaciones { get; set; }
        public required string EstadoEquipo { get; set; }
        public required string URLImagen { get; set; }

        //relacion hacia Loan
        public ICollection<LoanDTO> Prestamos { get; set; } = new List<LoanDTO>();

    }
}
