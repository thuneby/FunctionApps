using Microsoft.EntityFrameworkCore;

namespace OpenApiFunctions
{
    public class CosmosContext : DbContext
    {
        public CosmosContext(DbContextOptions<CosmosContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Product>().ToContainer("products")
                .HasNoDiscriminator().HasPartitionKey("Id");
        }

    }
}
