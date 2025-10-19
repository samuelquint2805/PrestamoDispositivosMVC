using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;


namespace PrestamoDispositivos.Services.Abstractions
{
    public interface IStudentService
    {

        public Task<Response<StudentDTO>> CreateStudentAsync(StudentDTO student);
        public Task<Response<StudentDTO>> UpdateStudentAsync(int id, StudentDTO student);

        public Task<Response<bool>> DeleteStudentAsync(int id);
        public Task<Response<StudentDTO>> GetStudentByIdAsync(int id);
        public Task<Response<List<StudentDTO>>> GetAllStudentsAsync();
    }
}
