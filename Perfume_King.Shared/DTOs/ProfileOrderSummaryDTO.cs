namespace Perfume_King.Shared.DTOs
{
    public class ProfileOrderSummaryDTO
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalItemsCount { get; set; }
        public List<ProfileOrderItemSummaryDTO> Items { get; set; } = [];
    }
}
