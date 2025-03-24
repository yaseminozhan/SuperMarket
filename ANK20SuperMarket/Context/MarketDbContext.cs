using ANK20SuperMarket.Models;
using Microsoft.EntityFrameworkCore;

namespace ANK20SuperMarket.Context
{
    public class MarketDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options) { }   
       
    }
}
