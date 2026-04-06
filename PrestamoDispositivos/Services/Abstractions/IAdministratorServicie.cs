using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface IAdministratorServicie
    {
        public Task<Response<AdministratorDTO>> CreateAdministratorAsync(AdministratorDTO Admindto);
        public Task<Response<AdministratorDTO>> UpdateAdministratorAsync(Guid id, AdministratorDTO Admindto);
        public Task<Response<bool>> DeleteAdministratorAsync(Guid id);
        public Task<Response<AdministratorDTO>> GetAdministratorByIdAsync(Guid id);
        public Task<Response<List<AdministratorDTO>>> GetAllAdministratorAsync();
    }
}
