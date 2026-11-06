using EStore.Application.Dtos;
namespace EStore.Application.Abstractions;

public interface IOrderRepository
{
    // Whole transaction (checking stock, decreasing, writing) lives in this method
    // under implementation of POST /orders
    Task<OrderDto> CreateAsync(string customer, IReadOnlyList<CreateOrderItemRequest> items, CancellationToken ct);
    
    // -||- GET orders/{id}
    Task<OrderDto?> GetByIdAsync(long id, CancellationToken ct);


}