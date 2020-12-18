using FactoryApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FactoryApi
{
    public sealed class ApplicationContext : IdentityDbContext<IdentityUser>
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

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole(FactoryApi.Roles.Administrator)
                    {NormalizedName = FactoryApi.Roles.Administrator.ToUpper()},
                new IdentityRole(FactoryApi.Roles.Reception)
                    {NormalizedName = FactoryApi.Roles.Reception.ToUpper()},
                new IdentityRole(FactoryApi.Roles.Writer)
                    {NormalizedName = FactoryApi.Roles.Writer.ToUpper()},
                new IdentityRole(FactoryApi.Roles.Printer)
                    {NormalizedName = FactoryApi.Roles.Printer.ToUpper()},
                new IdentityRole(FactoryApi.Roles.Issuer)
                    {NormalizedName = FactoryApi.Roles.Issuer.ToUpper()},
                new IdentityRole(FactoryApi.Roles.Board)
                    {NormalizedName = FactoryApi.Roles.Board.ToUpper()});
        }
    }
}