namespace Perfume_King.Shared.DTOs
{
    public enum Category
    {
        Men,
        Women,
        Unisex
    }
    public class PerfumeDTO
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
