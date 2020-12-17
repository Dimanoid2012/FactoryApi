using FactoryApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FactoryApi
{
    public sealed class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Color> Colors => Set<Color>();
        public DbSet<Size> Sizes => Set<Size>();
        public DbSet<Image> Images => Set<Image>();
        public DbSet<Model> Models => Set<Model>();
        public DbSet<ModelSize> ModelSizes => Set<ModelSize>();
        public DbSet<Order> Orders => Set<Order>();

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<ModelSize>().HasKey("ModelId", "SizeId");
            
        }
    }
}