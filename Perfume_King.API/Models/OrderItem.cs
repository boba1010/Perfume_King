namespace Perfume_King.API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderRequestId { get; set; }
        public int PerfumeId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
