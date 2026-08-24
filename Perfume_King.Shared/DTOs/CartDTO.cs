namespace Perfume_King.Shared.DTOs
{
    public class CartDTO
    {
        public int Id { get; set; }
        public List<CartItemDTO> CartItems { get; set; } = [];
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
