using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface IAppUser
    {
        public Task<Response<List<ApplicationUserDTO>>> GetAllUserUsAsync();
        public Task<Response<ApplicationUserDTO>> GetuserByIdAsync(Guid id);
        public Task<Response<ApplicationUserDTO>> UpdateUserUsAsync(Guid id, ApplicationUserDTO user);

        public Task<Response<bool>> DeleteSUserUsAsync(Guid id);
      
        
    }
}
