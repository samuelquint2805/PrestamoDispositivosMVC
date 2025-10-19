using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface ILoanService
    {
        public Task<Response<LoanDTO>> CreateStudentAsync(LoanDTO student);
        public Task<Response<LoanDTO>> UpdateStudentAsync(int id, LoanDTO student);

        public Task<Response<bool>> DeleteStudentAsync(int id);
        public Task<Response<LoanDTO>> GetStudentByIdAsync(int id);
        public Task<Response<List<LoanDTO>>> GetAllStudentsAsync();
    }
}
