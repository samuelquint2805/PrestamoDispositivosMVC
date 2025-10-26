namespace PrestamoDispositivos.DTO
{
    public class ToggleLoanStatusDTO
    {
        public Guid LoanId { get; set; }
        public Guid NewStatus { get; set; } 
    }
}
