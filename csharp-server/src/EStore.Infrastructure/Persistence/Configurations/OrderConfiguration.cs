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

/*
[GEN. BY DB VALEUS]

- We must tell EF HasDefaultValue so it knows that reading is from RETURNING after insert
- After Psql version 6, it maps DateTime with Kind=Utc so we are not tell explicit UtcNow


*/
}