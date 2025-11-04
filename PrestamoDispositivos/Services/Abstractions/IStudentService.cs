using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;


namespace PrestamoDispositivos.Services.Abstractions
{
    public interface IStudentService
    {

        public Task<Response<StudentDTO>> CreateStudentAsync(StudentDTO student);
        public Task<Response<StudentDTO>> UpdateStudentAsync(Guid id, StudentDTO student);

        public Task<Response<bool>> DeleteStudentAsync(Guid id);
        public Task<Response<StudentDTO>> GetStudentByIdAsync(Guid id);
        public Task<Response<List<StudentDTO>>> GetAllStudentsAsync();
    }
}
