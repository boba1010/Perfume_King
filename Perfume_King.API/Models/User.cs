namespace Perfume_King.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public byte[] Salt { get; set; } = null!;
        public string HashPassword { get; set; } = null!;
        public Cart Cart { get; set; } = null!;
    }
}
