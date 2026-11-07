namespace EStore.Application.Dtos;

public record CategoryStatsDto(string Category, int ProductCount, decimal AvgPrice, long TotalStock);
