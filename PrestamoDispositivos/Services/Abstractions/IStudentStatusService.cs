using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface IStudentStatusService
    {
        public Task<Response<studentStatusDTO>> CreateStudentStaAsync(studentStatusDTO studentStaDto);
        public Task<Response<studentStatusDTO>> UpdateStudentStaAsync(Guid id, studentStatusDTO studentStaDto);

        public Task<Response<bool>> DeleteStudentStatusAsync(Guid id);
        public Task<Response<studentStatusDTO>> GetStudentStaByIdAsync(Guid id);
        public Task<Response<List<studentStatusDTO>>> GetAllStudentStaAsync();
    }
}
