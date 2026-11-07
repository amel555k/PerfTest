namespace EStore.Application.Dtos;

public record PagedResult<T>(IReadOnlyList<T> Content, int Page, int Size, long TotalElements, int TotalPages);