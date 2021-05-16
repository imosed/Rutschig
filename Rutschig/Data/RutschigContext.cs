using Microsoft.EntityFrameworkCore;

namespace Rutschig.Models
{
    public class RutschigContext : DbContext
    {
        public RutschigContext(DbContextOptions<RutschigContext> options)
            : base(options)
        {
        }
        
        public DbSet<Alias> Aliases { get; set; }
    }
}