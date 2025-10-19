using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface IDeviceService
    {
        public Task<Response<deviceDTO>> CreateStudentAsync(deviceDTO student);
        public Task<Response<deviceDTO>> UpdateStudentAsync(int id, deviceDTO student);

        public Task<Response<bool>> DeleteStudentAsync(int id);
        public Task<Response<deviceDTO>> GetStudentByIdAsync(int id);
        public Task<Response<List<deviceDTO>>> GetAllStudentsAsync();
    }
}
