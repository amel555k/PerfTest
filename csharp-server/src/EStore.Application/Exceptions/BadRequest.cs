namespace EStore.Application.Exceptions;

public class BadRequestException(string message) : Exception(message);   // 400