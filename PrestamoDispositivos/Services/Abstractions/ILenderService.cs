using PrestamoDispositivos.Core;
using PrestamoDispositivos.DTO;

namespace PrestamoDispositivos.Services.Abstractions
{
    public interface ILenderService
    {
        public Task<Response<lenderDTO>> CreateLenderAsync(lenderDTO Lender);
        public Task<Response<lenderDTO>> UpdateLenderAsync(Guid id, lenderDTO Lender);

        public Task<Response<bool>> DeleteLenderAsync(Guid id);
        public Task<Response<lenderDTO>> GetLenderByIdAsync(Guid id);
        public Task<Response<List<lenderDTO>>> GetAllLendersAsync();
    }
}
