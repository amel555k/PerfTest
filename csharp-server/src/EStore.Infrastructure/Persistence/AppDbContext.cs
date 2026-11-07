using EStore.Domain.Entities;
using EStore.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace EStore.Infrastructure.Persistence;

internal sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());            
        
    }
}

/*
[AppDbContext multifunc.]

1. Unit of work - collects changes and sends them to SaveChanges()
2. Identity map - when you search Product with Id 5 twice, returns same object
3. Change tracker - remembers what you have changed

DbSet<> is suma suamrum repository

*/