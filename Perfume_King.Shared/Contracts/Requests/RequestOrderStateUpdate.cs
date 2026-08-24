using Perfume_King.Shared.DTOs;

namespace Perfume_King.Shared.Contracts.Requests
{
    public class RequestOrderStateUpdate
    {
        public int OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
