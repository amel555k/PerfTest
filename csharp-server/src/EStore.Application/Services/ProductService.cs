using System.Data.Common;
using EStore.Application.Abstractions;
using EStore.Application.Dtos;
using EStore.Application.Exceptions;

namespace EStore.Application.Services;

internal sealed class ProductService(IProductRepository products) : IProductService
{
    public Task<PagedResult<ProductDto>> GetPagedAsync(int page, int size, string? category, CancellationToken ct)
    {
        // crossroad funcion for validation
        ValidatePaging(page, size);
        return products.GetPagedAsync(page, size, category, ct);
    }

    public async Task<ProductDto> GetByIdAsync(long id, CancellationToken ct) => 
        await products.GetByIdAsync(id, ct) ?? throw new NotFoundException("Product not found");

    public Task<PagedResult<ProductDto>> SearchAsync(string? q, int page, int size, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(q))
            throw new BadRequestException("Validation failed: q must not be empty");

        return products.SearchAsync(q, page, size, ct);
    }

    public Task<IReadOnlyList<CategoryStatsDto>> GetCategoryStatsAsync(CancellationToken ct) =>
        products.GetCategoryStatsAsync(ct);


    // that crossroad function
     private static void ValidatePaging(int page, int size)
    {
        if (page < 0) throw new BadRequestException("Validation failed: page must be >= 0");
        if (size is < 1 or > 100) throw new BadRequestException("Validation failed: size must be between 1 and 100");
    }
}


/*
[Internal and Sealed]

Internal means that 
- Upper layers cannot call new ProductService(); (security)
- API layer can only see IProductService, not ProductService

Sealed means that
- no one can inherit that 
- JIT sometimes optimizes it more easialy



*/