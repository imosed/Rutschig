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

        public DbSet<Alias> AliasList { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alias>().HasKey(f => f.Id).HasName("PK_AliasId");
        }
    }
}
