using EStore.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Controllers;

[ApiController]
[Route("products")]
public class ProductsController(IProductService products) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 0, [FromQuery] int size = 20, 
        [FromQuery] string? category = null, CancellationToken ct = default) =>
        Ok(await products.GetPagedAsync(page, size, category, ct));

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? q, [FromQuery] int page = 0, [FromQuery] int size = 20,
        CancellationToken ct = default) =>
        Ok(await products.SearchAsync(q, page, size, ct));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct = default) =>
    Ok(await products.GetByIdAsync(id, ct));

}

/*
[ ROUTING ]
- [ApiController] does a lot of job automatically
    -- evaluates the source of parameters (route, query, body)
    -- automat. returns 400 when binding fails in format ProblemDetails
        --- API contract wants { "error": ...} so we override

*/