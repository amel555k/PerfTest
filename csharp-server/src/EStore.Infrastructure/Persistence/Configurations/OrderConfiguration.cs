using EStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EStore.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("orders");
        b.Property(o => o.Customer).HasMaxLength(255);
        b.Property(o => o.Total).HasPrecision(10, 2);
        b.Property(o => o.CreatedAt).HasDefaultValueSql("now()");
        b.HasMany(o => o.Items).WithOne().HasForeignKey(i => i.OrderId);
    }
}