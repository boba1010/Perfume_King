using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.DTOs;

namespace Perfume_King.Shared.Services
{
    public interface IOrderService
    {
        public Task<ServiceResult<List<OrderRequestDTO>>> GetOrdersAsync();
        public Task<ServiceResult<OrderRequestDTO>> GetOrderAsync(string orderNum);
        public Task<ServiceResult> RequestOrderAsync(PaymentMethod paymentMethod);

        public Task<ServiceResult<List<OrderRequestDTO>>> GetOrdersAdminAsync();
        public Task<ServiceResult<OrderRequestDTO>> GetOrderAdminAsync(string orderNumber);
        public Task<ServiceResult> UpdateOrderAsync(RequestOrderStateUpdate request);
    }
}
