using EStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EStore.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("products");
        b.Property(p => p.Name).HasMaxLength(255);
        b.Property(p => p.Category).HasMaxLength(100);
        b.Property(p => p.Price).HasPrecision(10, 2);
    }
}


/*
[FLUENT API VS ATTRBUTES]
- with attributes implementation, EF namespacewill be covered in Doamin layer too and that ruins zero depen.
- with naming convensions and zero migrations method, EF is clear mapper so there are not scheme testing per start


*/