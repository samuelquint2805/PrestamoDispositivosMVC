using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface ILoanService
    {
        Task<Response<LoanDTO>> CreateLoanAsync(LoanDTO loan);
        Task<Response<LoanDTO>> UpdateLoanAsync(Guid id, LoanDTO loan);
        Task<Response<bool>> DeleteLoanAsync(Guid id);
        Task<Response<LoanDTO>> GetLoanByIdAsync(Guid id);
        Task<Response<List<LoanDTO>>> GetAllLoansAsync();
        Task<Response<object>> ToggleLoanStatusAsync(ToggleLoanStatusDTO dto);
        Task<Response<List<StudentDTO>>> GetAllStudentsAsync();
        Task<Response<List<deviceDTO>>> GetAvailableDevicesAsync();
        Task<Response<List<deviceManagerDTO>>> GetAllAdministratorsAsync();
        Task<Response<List<LoanEventDTO>>> GetAllLoanEventsAsync();

        Task<Response<bool>> ReturnDeviceAsync(Guid loanId);
    }
}
