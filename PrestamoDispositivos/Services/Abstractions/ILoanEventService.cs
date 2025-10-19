using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface ILoanEventService
    {
        public Task<Response<LoanEventDTO>> CreateStudentAsync(LoanEventDTO student);
        public Task<Response<LoanEventDTO>> UpdateStudentAsync(int id, LoanEventDTO student);

        public Task<Response<bool>> DeleteStudentAsync(int id);
        public Task<Response<LoanEventDTO>> GetStudentByIdAsync(int id);
        public Task<Response<List<LoanEventDTO>>> GetAllStudentsAsync();
    }
}
