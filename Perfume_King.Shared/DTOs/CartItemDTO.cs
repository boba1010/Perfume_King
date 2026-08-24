namespace Perfume_King.Shared.DTOs
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int PerfumeId { get; set; }
        public int Quantity { get; set; }

        public string PerfumeName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public string ImageUri { get; set; } = null!;

        public decimal SubTotal => UnitPrice * Quantity;
    }
}
