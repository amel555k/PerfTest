namespace EStore.Application.Dtos;
public record CreateOrderRequest(string? Customer, List<CreateOrderItemRequest>? Items);