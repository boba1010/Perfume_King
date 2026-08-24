using Microsoft.EntityFrameworkCore;
using Perfume_King.API.Models;

namespace Perfume_King.API.DbContexts
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Perfume> Perfumes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OrderRequest> OrderRequests { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
