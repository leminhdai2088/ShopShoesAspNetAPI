﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using ShopShoesAPI.comment;
using ShopShoesAPI.common;
using ShopShoesAPI.order;
using ShopShoesAPI.product;
using ShopShoesAPI.user;

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

        public DbSet<PaymentEntity> PaymentEntities { get; set; }
        public DbSet<MerchantEntity> MerchantEntities { get; set; }
        public DbSet<PaymentDestinationEntity> PaymentDesEntities { get; set; }
        public DbSet<PaymentNotificationEntity> PaymentNotiEntities { get; set; }
        public DbSet<PaymentSignatureEntity> PaymentSigEntities { get; set; }
        public DbSet<PaymentTransactionEntity> PaymentTransEntities { get; set; }


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

            // User -> comments One to many
            modelBuilder.Entity<CommentEntity>()
                .HasOne(cm => cm.User)
                .WithMany(o => o.Comments)
                .HasForeignKey(cm => cm.UserId);

            // User -> Order One to many
            modelBuilder.Entity<OrderEntity>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<OrderEntity>()
                .Property(o => o.Total)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<PaymentNotificationEntity>()
                .HasOne(x => x.MerchantEntity)
                .WithMany(x => x.PaymentNotificationEntities)
                .HasForeignKey(x => x.NotiMerchantId);

            modelBuilder.Entity<PaymentNotificationEntity>()
                .HasOne(x => x.PaymentEntity)
                .WithMany(x => x.PaymentNotificationEntities)
                .HasForeignKey(x => x.NotiPaymentId);

            modelBuilder.Entity<PaymentEntity>()
                .HasOne(x => x.MerchantEntity)
                .WithMany(x => x.PaymentEntities)
                .HasForeignKey(x => x.MerchantId);

            modelBuilder.Entity<PaymentEntity>()
                .HasOne(x => x.PaymentDestinationEntity)
                .WithMany(x => x.PaymentEntities)
                .HasForeignKey(x => x.PaymentDesId);

            modelBuilder.Entity<PaymentTransactionEntity>()
                .HasOne(x => x.PaymentEntity)
                .WithMany(x => x.PaymentTransactionEntities)
                .HasForeignKey(x => x.PaymentId);

            modelBuilder.Entity<PaymentSignatureEntity>()
                .HasOne(x => x.PaymentEntity)
                .WithMany(x => x.PaymentSignatureEntities)
                .HasForeignKey(x => x.PaymentId);

            base.OnModelCreating(modelBuilder);

            SeedRoles(modelBuilder);
            SeedCategories(modelBuilder);
        }

        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin"},
                new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" }
                );
        }

        private static void SeedCategories(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryEntity>().HasData(
                new CategoryEntity() { Id  = 1, Name = "Category 1" },
                new CategoryEntity() { Id = 2, Name = "Category 2" },
                new CategoryEntity() { Id = 3, Name = "Category 3" },
                new CategoryEntity() { Id = 4, Name = "Category 4" },
                new CategoryEntity() { Id = 5, Name = "Category 5" }
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
