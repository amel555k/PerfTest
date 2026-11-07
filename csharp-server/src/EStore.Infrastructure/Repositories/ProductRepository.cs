using System.Drawing;
using System.Linq.Expressions;
using EStore.Application.Abstractions;
using EStore.Application.Dtos;
using EStore.Domain.Entities;
using EStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions;

namespace EStore.Infrastructure.Repositories;

internal sealed class ProductRepository(AppDbContext db) : IProductRepository
{
    // Projection is written once: EF makes SELECT only with nessecary columns from it
    private static readonly Expression<Func<Product, ProductDto>> ToDto =
        p => new ProductDto(p.Id, p.Name, p.Description, p.Category, p.Price, p.Stock);

    // only task fowarding
    public Task<PagedResult<ProductDto>> GetPagedAsync(int page, int size, string? category, CancellationToken ct)
    {
        var query = db.Products.AsNoTracking();
        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category == category);
        
        return ToPageAsync(query, page, size, ct);
    }

    public Task<ProductDto?> GetByIdAsync(long id, CancellationToken ct) =>
        db.Products.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(ToDto)
            .FirstOrDefaultAsync(ct);

    public Task<PagedResult<ProductDto>> SearchAsync(string q, int page, int size, CancellationToken ct)
    {
        var pattern = $"%{EscapeLike(q)}";
        var query = db.Products.AsNoTracking()
        // ILIKE FROM SQL, this means that if description or name mathces with pattern, push that row to result
            .Where(p => EF.Functions.ILike(p.Name, pattern) || EF.Functions.ILike(p.Description!, pattern)); 

        return ToPageAsync(query, page, size, ct);
    }

    public async Task<IReadOnlyList<CategoryStatsDto>> GetCategoryStatsAsync(CancellationToken ct)
    {
        var rows = await db.Products.AsNoTracking()

            // GROUP BY CLAUSE
            .GroupBy(p => p.Category)

            // SELECT query with custom columns
            .Select(g => new
            {
                Category = g.Key,
                Count = g.Count(),
                Avg = g.Average(p => p.Price),
                Stock = g.Sum(p => (long)p.Stock)   // SUM on int gives BIGINT
            })

            // ORDER BY
            .OrderBy(x => x.Category)

            // result push to list
            .ToListAsync(ct);

            return rows

            // further formation via SELECT
            .Select( r => new CategoryStatsDto(r.Category, r.Count,
                Math.Round(r.Avg, 2, MidpointRounding.AwayFromZero), r.Stock))
                .ToList();
    }

    private static async Task<PagedResult<ProductDto>> ToPageAsync(
        IQueryable<Product> query, int page, int size, CancellationToken ct)
    {
        var total = await query.LongCountAsync(ct);
        var offset = (long)page * size;

        // Page after end: do not spend second query( and evadin int overflow to skip)
        var content = offset >= total
            ? new List<ProductDto>()
            : await query.OrderBy(p => p.Id).Skip((int)offset).Take(size).Select(ToDto).ToListAsync(ct);

        var totalPages = (int)((total + size -1) / size);

        return new PagedResult<ProductDto>(content, page, size, total, totalPages);
    }

    // Contract said LIKE '%q%' so we q treat like classic text (% i _ do not have special meaning)
    private static string EscapeLike(string s) =>
        s.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
    

}