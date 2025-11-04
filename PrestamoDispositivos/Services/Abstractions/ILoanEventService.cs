using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface ILoanEventService
    {
        public Task<Response<LoanEventDTO>> CreateLoanEventoAsync(LoanEventDTO student);
        public Task<Response<LoanEventDTO>> UpdateLoanEventoAsync(Guid id, LoanEventDTO student);

        public Task<Response<bool>> DeleteLoanEventoAsync(Guid id);
        public Task<Response<LoanEventDTO>> GetLoanEventoByIdAsync(Guid id);
        public Task<Response<List<LoanEventDTO>>> GetAllLoanEventoAsync();
    }
}
