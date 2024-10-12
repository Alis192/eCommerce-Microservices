using Microsoft.EntityFrameworkCore;
using SearchService.Models;

namespace SearchService.Data
{
    public class SearchDbContext : DbContext
    {
        public SearchDbContext(DbContextOptions<SearchDbContext> options) : base(options) { }
        
        public DbSet<Product> Products { get; set; }
    }
}
