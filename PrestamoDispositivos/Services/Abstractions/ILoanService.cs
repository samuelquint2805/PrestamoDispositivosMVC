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
        Task<Response<List<StudentDTO>>> GetAllStudentsAsync();
        Task<Response<List<deviceDTO>>> GetAvailableDevicesAsync();
        Task<Response<List<AdministratorDTO>>> GetAllAdministratorsAsync();
        Task<Response<List<LoanDTO>>> GetAllLoansPerStudentAsync(Guid id);
        Task<Response<bool>> ReturnDeviceAsync(Guid loanId);
    }
}
