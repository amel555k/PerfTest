using EStore.Application.Dtos;
namespace EStore.Application.Abstractions;

public interface IOrderService
{
    Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken ct);
    Task<OrderDto> GetByIdAsync(long id, CancellationToken ct);
}