using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopShoesAPI.common;
using ShopShoesAPI.product;
using ShopShoesAPI.user;
using System.Threading;

namespace ShopShoesAPI.Data
{
    public class MyDbContext: IdentityDbContext<UserEnityIndetity>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        //public DbSet<UserEnityIndetity> UserEnityIndetity { get; set; }
        public DbSet<ProductEntity> ProductEntity { get; set; }
        public DbSet<CategoryEntity> CategoryEntity { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEnityIndetity>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<UserEnityIndetity>()
                .HasIndex(e => e.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<CategoryEntity>()
                .HasMany(e => e.Products)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .HasPrincipalKey(e => e.Id);

            base.OnModelCreating(modelBuilder);

            SeedRoles(modelBuilder);
        }

        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin"},
                new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" }
                );
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow; // current datetime

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedAt = now;
                }
                ((BaseEntity)entity.Entity).UpdatedAt = now;
            }
        }
    }
}
