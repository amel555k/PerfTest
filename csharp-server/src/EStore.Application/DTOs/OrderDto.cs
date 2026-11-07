namespace EStore.Application.Dtos;

public record OrderDto(long Id, string Customer, decimal Total, DateTime CreatedAt, IReadOnlyList<OrderItemDto> Items);