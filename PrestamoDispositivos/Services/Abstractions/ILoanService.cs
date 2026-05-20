using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface ILoanService
    {
        Task<Response<List<LoanDTO>>> GetAllLoansAsync();
        Task<Response<List<LoanDTO>>> GetLoansByUserAsync(Guid idUser);
        Task<Response<List<LoanDTO>>> GetAllLoansPerStudentAsync(Guid idEst);
        Task<Response<LoanDTO>> GetLoanByIdAsync(Guid id);
        Task<Response<LoanDTO>> CreateLoanAsync(LoanDTO dto);
        Task<Response<LoanDTO>> UpdateLoanAsync(Guid id, LoanDTO dto);
        Task<Response<bool>> DeleteLoanAsync(Guid id);
        Task<Response<bool>> ReturnDeviceAsync(Guid loanId);
        Task<Response<List<StudentDTO>>> GetAllStudentsAsync();
        Task<Response<List<deviceDTO>>> GetAvailableDevicesAsync();
        Task<Response<List<AdministratorDTO>>> GetAllAdministratorsAsync();

    }
}
