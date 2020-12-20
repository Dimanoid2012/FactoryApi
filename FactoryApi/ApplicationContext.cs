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
            builder.Entity<Color>().OwnsOne(x => x.RGB);
            builder.Entity<Order>().Property(x => x.Number).ValueGeneratedOnAdd();

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole(FactoryApi.Roles.Administrator)
                {
                    Id = "88b5386c-1904-4326-97f7-14a497549c49",
                    NormalizedName = FactoryApi.Roles.Administrator.ToUpper(),
                    ConcurrencyStamp = "7ef61046-db40-41dc-ae21-4c78e722b0a0"
                },
                new IdentityRole(FactoryApi.Roles.Reception)
                {
                    Id = "f8c8bc95-23f8-45ba-8b2d-88352bfd3289",
                    NormalizedName = FactoryApi.Roles.Reception.ToUpper(),
                    ConcurrencyStamp = "2d64bc21-9f70-41b6-b8fe-8af59df380d9"
                },
                new IdentityRole(FactoryApi.Roles.Writer)
                {
                    Id = "41e5abbc-202c-4b65-bc48-a8ae8a14722f",
                    NormalizedName = FactoryApi.Roles.Writer.ToUpper(),
                    ConcurrencyStamp = "07364805-b6b5-49fd-8c47-aa626cfd4610"
                },
                new IdentityRole(FactoryApi.Roles.Printer)
                {
                    Id = "c5872579-9861-4889-a89e-dbce38c0134d",
                    NormalizedName = FactoryApi.Roles.Printer.ToUpper(),
                    ConcurrencyStamp = "875a734f-5b08-42ef-a1ba-ea9d1f15a7d0"
                },
                new IdentityRole(FactoryApi.Roles.Issuer)
                {
                    Id = "b5a01fe8-225a-4bc8-aa91-cb17305b80f9",
                    NormalizedName = FactoryApi.Roles.Issuer.ToUpper(),
                    ConcurrencyStamp = "f13a9186-1bac-4884-b6e9-307e71f72999"
                },
                new IdentityRole(FactoryApi.Roles.Board)
                {
                    Id = "84f1aac4-d856-4839-853b-e62c49867d7e",
                    NormalizedName = FactoryApi.Roles.Board.ToUpper(),
                    ConcurrencyStamp = "3f4857ec-5896-49cb-90da-695d21f33b76"
                });
        }
    }
}