namespace Perfume_King.Shared.DTOs
{
    public enum OrderStatus
    {
        Pending,
        Completed,
        Cancelled,
        OutForDelivery,
        Delivered,
        Processing
    }

    public enum PaymentMethod
    {
        COD
    }

    public class OrderRequestDTO
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = "Cairo";
        public List<OrderItemDTO> Items { get; set; } = [];
        public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
    }
}
