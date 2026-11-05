namespace EStore.Application.Dtos;

public record ProductDto(long Id, string Name, string? Description, string Category, decimal Price, int Stock);