namespace Perfume_King.API.Models
{
    public enum OrderStatus
    {
        Pending,
        Shipped,
        Completed,
        Cancelled,
        OutForDelivery,
        Delivered,
        Processing
    }

    public enum PaymentMethod
    {
        COD,

    }

    public class OrderRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = "Cairo";
        public List<OrderItem> Items { get; set; } = [];
        public decimal TotalPrice => Items.Sum(i => i.Price * i.Quantity);
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; }
    }
}
