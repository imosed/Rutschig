using Microsoft.EntityFrameworkCore;
using Rutschig.Models;

namespace Rutschig.Data
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