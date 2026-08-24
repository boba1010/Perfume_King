namespace Perfume_King.API.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItem> CartItems { get; set; } = [];
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
