using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface IDeviceService
    {
        public Task<Response<deviceDTO>> CreateDeviceAsync(deviceDTO student);
        public Task<Response<deviceDTO>> UpdateDeviceAsync(Guid id, deviceDTO student);

        public Task<Response<bool>> DeleteDeviceAsync(Guid id);
        public Task<Response<deviceDTO>> GetDeviceByIdAsync(Guid id);
        public Task<Response<List<deviceDTO>>> GetAllDeviceAsync();
    }
}
