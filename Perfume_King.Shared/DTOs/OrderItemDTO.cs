namespace Perfume_King.Shared.DTOs
{
    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int OrderRequestId { get; set; }
        public int PerfumeId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
