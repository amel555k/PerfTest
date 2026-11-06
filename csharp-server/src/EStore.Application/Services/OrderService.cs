using EStore.Application.Abstractions;
using EStore.Application.Exceptions;
using EStore.Application.Dtos;

namespace EStore.Application.Services;

internal sealed class OrderService(IOrderRepository orders) : IOrderService
{
    private const int MaxCustomerLength = 255; // varchar(255) in db

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct)
    {
        Validate(request);
        return await orders.CreateAsync(request.Customer!, request.Items!, ct);
    }

    public async Task<OrderDto> GetByIdAsync(long id, CancellationToken ct) =>
        await orders.GetByIdAsync(id, ct) ?? throw new NotFoundException("Order not found");

    private static void Validate(CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Customer))
            throw new BadRequestException("Validation failed: customer must not be empty");
        if (request.Customer.Length > MaxCustomerLength)
            throw new BadRequestException("Validation failed: customer is too long");
        if (request.Items is null or { Count: 0 })
            throw new BadRequestException("Validation failed: items must not be emtpy");

        var seen = new HashSet<long>();
        foreach(var item in request.Items)
        {
            if (item.Quantity < 1)
                throw new BadRequestException("Validatin failed: quantity must be >= 1");
            if(!seen.Add(item.ProductId))
                throw new BadRequestException($"Valiadtion failed: duplicate ProductId: {item.ProductId}");
        }
    }
}