using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface IStudentStatusService
    {
        public Task<Response<studentStatusDTO>> CreateStudentAsync(studentStatusDTO student);
        public Task<Response<studentStatusDTO>> UpdateStudentAsync(int id, studentStatusDTO student);

        public Task<Response<bool>> DeleteStudentAsync(int id);
        public Task<Response<studentStatusDTO>> GetStudentByIdAsync(int id);
        public Task<Response<List<studentStatusDTO>>> GetAllStudentsAsync();
    }
}
