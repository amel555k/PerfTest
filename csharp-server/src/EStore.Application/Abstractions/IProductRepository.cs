using EStore.Application.Dtos;
namespace EStore.Application.Abstractions;


public interface IProductRepository
{
    Task<PagedResult<ProductDto>> GetPagedAsync(int page, int size, string? category, CancellationToken ct);
    Task<ProductDto?> GetByIdAsync(long id, CancellationToken ct);
    Task<PagedResult<ProductDto>> SearchAsync(string q, int page, int size, CancellationToken ct);
    Task<IReadOnlyList<CategoryStatsDto>> GetCategoryStatsAsync(CancellationToken ct);
}


/*
[Cancellation Token]

- if client breaks the request, ASP.NET signals with token, then EF interrupts query to the DB
- Canc token is placed througth all layer to 'ToListAsync(ct)' 
- Without that, server continues to do the job for no one (resource and performance cost) 

[Why repository interface?]

Two reasons:
- Application is left without EF dependencies (Continuity of Dep. Inversion)
- and theres border od layers


*/

