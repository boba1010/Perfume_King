namespace Perfume_King.API.Models
{
    public enum Category
    {
        Men,
        Women,
        Unisex
    }

    public class Perfume
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUri { get; set; } = null!;
        public int SaleNumber { get; set; }
        public Category Category { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
