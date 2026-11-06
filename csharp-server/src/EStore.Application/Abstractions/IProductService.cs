using EStore.Application.Dtos;
namespace EStore.Application.Abstractions;


public interface IProductService
{
    Task<PagedResult<ProductDto>> GetPagedAsync(int page, int size, string? category, CancellationToken ct);
    Task<ProductDto> GetByIdAsync(long id, CancellationToken ct);
    Task<PagedResult<ProductDto>> SearchAsync(string? q, int page, int size, CancellationToken ct);
    Task<IReadOnlyList<CategoryStatsDto>> GetCategoryStatsAsync(CancellationToken ct);

}