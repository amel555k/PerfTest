namespace EStore.Application.Dtos;

public record CreateOrderItemRequest(long ProductId, int Quantity);