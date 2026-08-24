namespace Perfume_King.API.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int PerfumeId { get; set; }
        public int Quantity { get; set; }
    }
}
