using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;
using PrestamoDispositivos.Services.Abstractions;


namespace PrestamoDispositivos.Services.Implementations
{
    public class StudentService : IStudentService
    {
        public Task<Response<StudentDTO>> CreateStudentAsync(StudentDTO student)
        {
            throw new NotImplementedException();
        }

        public Task<Response<bool>> DeleteStudentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<List<StudentDTO>>> GetAllStudentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Response<StudentDTO>> GetStudentByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<StudentDTO>> UpdateStudentAsync(int id, StudentDTO student)
        {
            throw new NotImplementedException();
        }
    }
}
