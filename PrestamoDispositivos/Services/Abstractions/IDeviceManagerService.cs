using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface IDeviceManagerService
    {
        public Task<Response<deviceManagerDTO>> CreateDeviceManagerAsync(deviceManagerDTO devicManDto);
        public Task<Response<deviceManagerDTO>> UpdateDeviceManagerAsync(Guid id, deviceManagerDTO student);

        public Task<Response<bool>> DeleteDeviceManagerAsync(Guid id);
        public Task<Response<deviceManagerDTO>> GetDeviceManagerByIdAsync(Guid id);
        public Task<Response<List<deviceManagerDTO>>> GetAllDeviceManagerAsync();
    }
}
