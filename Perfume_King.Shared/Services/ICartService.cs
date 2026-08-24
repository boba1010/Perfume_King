using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.DTOs;

namespace Perfume_King.Shared.Services
{
    public interface ICartService
    {
        public Task<ServiceResult> AddPerfumeToCartAsync(int id);
        public Task<ServiceResult<CartDTO>> GetCartAsync();
        public Task<ServiceResult> RemoveCartItemAsync(int id);
        public Task<ServiceResult> ChangeCartItemQtyAsync(RequestCartItemQtyChange request);
    }
}
