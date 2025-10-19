using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface IDeviceManagerService
    {
        public Task<Response<deviceManagerDTO>> CreateStudentAsync(deviceManagerDTO devicManDto);
        public Task<Response<deviceManagerDTO>> UpdateStudentAsync(int id, deviceManagerDTO student);

        public Task<Response<bool>> DeleteStudentAsync(int id);
        public Task<Response<deviceManagerDTO>> GetStudentByIdAsync(int id);
        public Task<Response<List<deviceManagerDTO>>> GetAllStudentsAsync();
    }
}
