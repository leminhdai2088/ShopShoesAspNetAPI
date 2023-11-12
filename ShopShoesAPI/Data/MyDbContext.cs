using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopShoesAPI.comment;
using ShopShoesAPI.common;
using ShopShoesAPI.order;
using ShopShoesAPI.product;
using ShopShoesAPI.user;
using System.Threading;

namespace ShopShoesAPI.Data
{
    public class MyDbContext: IdentityDbContext<UserEnityIndetity>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        //public DbSet<UserEnityIndetity> UserEnityIndetity { get; set; }
        public DbSet<CategoryEntity> CategoryEntities { get; set; }
        public DbSet<ProductEntity> ProductEntities { get; set; }
        public DbSet<CommentEntity> CommentEntities { get; set; }
        public DbSet<OrderEntity> OrderEntities { get; set; }
        public DbSet<OrderDetailEntity> OrderDetailEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique Email user
            modelBuilder.Entity<UserEnityIndetity>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // Unique Phone user
            modelBuilder.Entity<UserEnityIndetity>()
                .HasIndex(e => e.PhoneNumber)
                .IsUnique();

            // Product -> Category One to many
            modelBuilder.Entity<CategoryEntity>()
                .HasMany(e => e.Products)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .HasPrincipalKey(e => e.Id);

            // Product -> Comment One to many
            modelBuilder.Entity<ProductEntity>()
                .HasMany(e => e.Comments)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId)
                .HasPrincipalKey(e => e.Id);

            // Primary key order detail
            modelBuilder.Entity<OrderDetailEntity>().HasKey(od => new {od.OrderId, od.ProductId });

            // Order -> OrderDetail One to many
            modelBuilder.Entity<OrderDetailEntity>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            // Product -> OrderDetail One to many
            modelBuilder.Entity<OrderDetailEntity>()
                .HasOne(od => od.Product)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.ProductId);

            // User -> Order One to many
            modelBuilder.Entity<OrderEntity>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);
              

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
