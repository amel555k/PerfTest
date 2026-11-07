using EStore.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Controllers;

[ApiController]
[Route("stats")]
public class StatsController(IProductService products) : ControllerBase
{
    [HttpGet("categories")]
    public async Task<IActionResult> Categories(CancellationToken ct = default) =>
        Ok(await products.GetCategoryStatsAsync(ct));
} 