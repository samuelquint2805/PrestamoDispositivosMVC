using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Services.Implementations
{
    public class StudentStatusService : IStudentStatusService
    {
        public Task<Response<studentStatusDTO>> CreateStudentAsync(studentStatusDTO student)
        {
            throw new NotImplementedException();
        }

        public Task<Response<bool>> DeleteStudentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<List<studentStatusDTO>>> GetAllStudentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Response<studentStatusDTO>> GetStudentByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<studentStatusDTO>> UpdateStudentAsync(int id, studentStatusDTO student)
        {
            throw new NotImplementedException();
        }
    }
}
