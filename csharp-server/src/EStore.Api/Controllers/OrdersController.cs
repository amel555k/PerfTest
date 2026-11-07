using EStore.Application.Abstractions;
using EStore.Application.Dtos;
using EStore.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(IOrderService orders) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct = default)
    {
        var order = await orders.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct = default) =>
        Ok(await orders.GetByIdAsync(id, ct));
}
